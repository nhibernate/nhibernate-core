rem Open the file "build\build.log" to read NAnt's output
if not exist build   md build
NAnt   -D:build.debug=false   clean  package   > build\build.log
