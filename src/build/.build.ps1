<#
.Synopsis
	Build script (https://github.com/nightroman/Invoke-Build)
#>

[cmdletBinding()]
param(
	$appName = "ChromeRuntimeDownloader",
	$toolsDir = (Join-Path $BL.ScriptsPath  "tools"),
	$scriptsPath = $BL.ScriptsPath,
	$nuget = (Join-Path $toolsDir  "nuget/nuget.exe"),
	$libz = (Join-Path $toolsDir  "LibZ.Tool/tools/libz.exe"),
	$7zip = (Join-Path $toolsDir  "7-Zip.CommandLine/tools/7za.exe"),
	$srcDir = (Join-Path $BL.RepoRoot "src"),
	$project  = (Join-Path $BL.RepoRoot  "/src/ChromeRuntimeDownloader/ChromeRuntimeDownloader.csproj" ),
	$sln  = (Join-Path $BL.RepoRoot  "/src/ChromeRuntimeDownloader.sln" ),
	$buildTmpDir  = (Join-Path $BL.BuildOutPath "tmp" ),

	$buildReadyDir  = (Join-Path $BL.BuildOutPath "ready" ),
	$Dirs = (@{"marge" = "marge"; "runtime" = "runtime"; "build" = "build"; "nuget" = "nuget"; "main" = "main"  }),
	$buildWorkDir  = (Join-Path $buildTmpDir "build" ),
	$target  = "Release"

)





# Msbuild 
Set-Alias MSBuild (Resolve-MSBuild)

# inser tools
. (Join-Path $BL.ScriptsPath  "vendor\ps-auto-helpers\ps\misc.ps1")
. (Join-Path $BL.ScriptsPath  "vendor\ps-auto-helpers\ps\io.ps1")
. (Join-Path $BL.ScriptsPath  "vendor\ps-auto-helpers\ps\assembly-tools.ps1")




# Synopsis: Package-Restore
task RestorePackage {

	 Set-Location   $BL.RepoRoot
	"Restore packages: Sln: {$sln}"
	exec {  &$nuget restore $sln  }
}

task Startup-TeamCity {

	if ($env:TEAMCITY_VERSION) {
		$tvc = $env:TEAMCITY_VERSION
		Write-Build Green "Setup TeamCity: $tvc" 
		$s = $BL.BuildVersion.SemVer
		"##teamcity[buildNumber '$s']"
		try {
			$max = $host.UI.RawUI.MaxPhysicalWindowSize
			if($max) {
			$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(9999,9999)
			$host.UI.RawUI.WindowSize = New-Object System.Management.Automation.Host.Size($max.Width,$max.Height)
		}
		} catch {}
	}

}

task CheckTools {
	Write-Build Green "Check: Nuget"
	DownloadIfNotExists "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" $nuget 
	DownloadNugetIfNotExists $nuget "LibZ.Tool" $toolsDir $libz
	DownloadNugetIfNotExists $nuget "7-Zip.CommandLine" $toolsDir $7zip

}

# Synopsis: Build the project.
task Build {
	
	Write-Build Green "*** Build *** "
	$p = $project 
	$out = (Join-Path $buildTmpDir $Dirs.build  )
	try {

		EnsureDirExistsAndIsEmpty $buildWorkDir

		"Build; Project file: $p"
		"Build; out dir: $out"
		"Build; Target: $target"

		$bv = $BL.BuildVersion

		"AssemblyVersion: $($bv.AssemblyVersion)"
		"AssemblyVersion: $($bv.AssemblyFileVersion)"
		"AssemblyVersion: $($bv.AssemblyInformationalVersion)"

		$srcWorkDir = Join-Path $srcDir "ChromeRuntimeDownloader"
		BackupTemporaryFiles $srcWorkDir  "Properties\AssemblyInfo.cs"
		UpdateAssemblyInfo $srcWorkDir $bv.AssemblyVersion $bv.AssemblyFileVersion $bv.AssemblyInformationalVersion $appName "DenebLab" "DenebLab"
		exec { MSBuild $p  /v:quiet  /p:Configuration=$target /p:OutDir=$out   } 
	}

	catch {
		RestoreTemporaryFiles $srcWorkDir
		throw $_.Exception
		exit 1
	}
	finally {
		RestoreTemporaryFiles $srcWorkDir
	}
}




# Synopsis: Marge 
task Marge  {	

	Write-Build Green "*** Marge ***"

	$buildDir = (Join-Path $buildTmpDir $Dirs.build  )
	$margedDir = (Join-Path $buildTmpDir $Dirs.marge  )

	Set-Location  $buildDir
	EnsureDirExistsAndIsEmpty $margedDir 

	$dlls = [System.IO.Directory]::GetFiles($buildDir, "*.dll")
	$exclude = $donotMarge | Foreach-Object { "--exclude=$_" }

	foreach ($f in  $dlls ){
		Copy-Item $f -Destination $margedDir
	}
	
	Copy-Item "ChromeRuntimeDownloader.exe"  -Destination $margedDir 

	Set-Location  $margedDir
	& $libz inject-dll --assembly "ChromeRuntimeDownloader.exe" --include *.dll  $exclude --move

}




# Synopsis: Make nuget file
task Make-Nuget  {

	 "*** Make-Nuget  ***"
	$margedDir = (Join-Path $buildTmpDir $Dirs.marge  )
	$nugetDir = (Join-Path $buildTmpDir $Dirs.nuget  )
	$mainDir = (Join-Path $nugetDir $Dirs.main  )
	$syrupDir = Join-Path $nugetDir "/_syrup"
	$syruScriptspDir = Join-Path $syrupDir "scripts"

	EnsureDirExistsAndIsEmpty $nugetDir
	EnsureDirExistsAndIsEmpty $mainDir
	EnsureDirExistsAndIsEmpty $syrupDir
	EnsureDirExistsAndIsEmpty $syruScriptspDir
	
	$src =  "$margedDir/*"
	$dst = $mainDir
	"Copy main; Src: $src ; Dst: $dst"
	Copy-Item  $src  -Recurse  -Destination $dst


	



	$spacFilePath = Join-Path $scriptsPath "nuget\nuget.nuspec"
	$specFileOutPath = Join-Path $nugetDir "$appName.nuspec"
	
    
    $spec = [xml](get-content $spacFilePath)
    $spec.package.metadata.version = ([string]$spec.package.metadata.version).Replace("{Version}", $BL.BuildVersion.SemVer)
    $spec.Save($specFileOutPath )

	$readyDir =  Join-Path $buildReadyDir  $Dirs.nuget
	EnsureDirExistsAndIsEmpty $readyDir
    exec { &$nuget pack $specFileOutPath -OutputDirectory $readyDir  -NoPackageAnalysis}
	$nugetFile =  ([System.IO.Directory]::GetFiles($readyDir , "*.nupkg"))[0]

}



task Publish-Local -If (-not   $env:TEAMCITY_VERSION ) {
    "Publish local"
	$devDir = (Join-Path $BL.RepoRoot ".dev")
	$standolone = (Join-Path $devDir  "standalone-run")
	$workDir = (Join-Path $devDir  "work-dir")

	
	
	



	"Make standalone app"
	EnsureDirExistsAndIsEmpty  $standolone 
	$margedDir = (Join-Path $buildTmpDir $Dirs.marge  )
	$src = "$margedDir/*"
	$dst = $standolone 
	"Copy main; Src: $src ; Dst: $dst"
	Copy-Item  $src  -Recurse  -Destination $dst


	



}

task Publish-TeamCity -If ($env:TEAMCITY_VERSION ) {
	"Publish teamcity"
	$runDir = Split-Path -Path $runtimeInfoFile
	EnsureDirExists $runDir
	if(-not (RuntimeFileIsValid) ){ 
		"Copy new runtime;"
		$readyDir =  Join-Path $buildReadyDir  $Dirs.runtime
		$arch = @("x86","x64")
		foreach ($a in  $arch) {
				$src = "$readyDir/runtime.$a.$runtimeVersion.zip"
			    $dst =  $runDir
				"Copy; Src:$src ; Dst:  $dst"
				Copy-Item $src  -Destination  $dst
		}
		$o = [PSCustomObject] @{ChromeRuntimeVersion = $runtimeVersion }
		$json = (ConvertTo-json $o)		
		[System.IO.File]::WriteAllLines($runtimeInfoFile, $json, [text.encoding]::UTF8)
	}

	$serverAppDir = Join-Path $serverDir $appName 
	EnsureDirExists $serverAppDir
	$syrupDir = Join-Path $serverAppDir "syrup"
	EnsureDirExists $syrupDir
	
	$readyDir =  Join-Path $buildReadyDir  $Dirs.nuget

	$src = "$readyDir/*"
	$dst = $syrupDir
	"Copy to syrup; Src:$src ; Dst: $dst"
	Copy-Item $src  -Destination  $dst

}


# Synopsis: Remove temp files.
task Clean {
 #   Remove-Item bin, obj -Recurse -Force -ErrorAction 0
}


function DownloadIfNotExists($src , $dst){

	If (-not (Test-Path $dst)){
		$dir = [System.IO.Path]::GetDirectoryName($dst)
		If (-not (Test-Path $dir)){
			New-Item -ItemType directory -Path $dir
		}
	 	Invoke-WebRequest $src -OutFile $dst
	} 
}



# Synopsis: Build and clean.

task Startup  Startup-TeamCity, CheckTools
task BuildTask RestorePackage,  Build
task Publish Publish-Local, Publish-TeamCity
task . Startup, BuildTask, Marge, Make-Nuget,  Publish
