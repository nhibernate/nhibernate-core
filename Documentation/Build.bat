rem Open the file "build\build.log" to read NAnt's output
if not exist build	md build
NAnt   -D:vshik.installed=true   clean  build   > build\build.log
