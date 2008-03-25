rem Open the file "build\build.log" to read NAnt's output
if not exist build   md build
NAnt   clean-src  build   > build\build.log
