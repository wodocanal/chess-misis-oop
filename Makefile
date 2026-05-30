deps:
	sudo pacman -S dotnet-sdk

build:
	dotnet build chess/Chess.sln

run:
	./launch.sh

clean:
	dotnet clean chess/Chess.sln

fmt:
	dotnet format chess/Chess.sln