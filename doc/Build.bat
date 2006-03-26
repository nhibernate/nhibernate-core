rem Open the file "build\build.log" to read NAnt's output
md build
NAnt -buildfile:reference\Documentation.build  -D:vshik.installed=true   clean  build> build\build.log
