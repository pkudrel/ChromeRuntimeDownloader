& git rev-parse --show-toplevel
& git log --max-count=1 --pretty=format:%cI HEAD
& git rev-parse --abbrev-ref HEAD
& git symbolic-ref --short -q HEAD
& git for-each-ref refs/tags --sort=-taggerdate --format='%(refname:short)' --count=1
& git rev-parse --show-toplevel

