.PHONY: all
all: debug

debug: BarkditorGui.Presentation/**/*
	rm -rf BarkditorGui.Presentation/bin/Debug/net6.0/resources

	dotnet build

	mkdir -p BarkditorGui.Presentation/bin/Debug/net6.0/resources
	cp -r BarkditorGui.Presentation/Pic BarkditorGui.Presentation/bin/Debug/net6.0/resources
	cp -r BarkditorGui.Presentation/Css BarkditorGui.Presentation/bin/Debug/net6.0/resources
	cp -r BarkditorGui.Presentation/Glade BarkditorGui.Presentation/bin/Debug/net6.0/resources


release: BarkditorGui.Presentation/**/*
	rm -rf BarkditorGui.Presentation/bin/Release/net6.0/resources

	dotnet build -c Release

	mkdir -p BarkditorGui.Presentation/bin/Release/net6.0/resources
	cp -r BarkditorGui.Presentation/Pic BarkditorGui.Presentation/bin/Release/net6.0/resources
	cp -r BarkditorGui.Presentation/Css BarkditorGui.Presentation/bin/Release/net6.0/resources
	cp -r BarkditorGui.Presentation/Glade BarkditorGui.Presentation/bin/Release/net6.0/resources

run:
	cd BarkditorGui.Presentation/bin/Debug/net6.0; ./BarkditorGui.Presentation
