echo "promote.bat: %CD%"
echo "promote.bat: copy %1 dll to %2/RailHexLib.dll"
cp bin/%1/netstandard2.0/RailHexLib.dll %2/RailHexLib.dll
cp bin/%1/netstandard2.0/RailHexLib.pdb %2/RailHexLib.pdb
