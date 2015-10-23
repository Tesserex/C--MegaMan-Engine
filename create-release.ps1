param([string]$v, [switch]$debug)
$path = if ($debug) { "Debug" } else { "Release" }
md -Force Releases/$v

Copy-Item "Mega Man/bin/x86/$path" Releases/$v/Engine -recurse

Copy-Item "WPFEditor/bin/$path" Releases/$v/Editor -recurse

Get-ChildItem Releases/$v -r -include *.pdb -Force | Remove-Item -r -Force
Get-ChildItem Releases/$v -r -include *.vshost.* -Force | Remove-Item -r -Force

Copy-Item "Demo Project" "Releases/$v" -recurse

Write-Zip Releases\$v Releases\CME-$v.zip