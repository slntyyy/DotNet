# dotnetcore

## dotnet add
### webapi
### classlib
### console

## dotnet method
``` 
dotnet build
dotnet run
dotnet add referennce ../{test}.csproj
dotnet add package Newtonsoft.json
dotnet new classlib
```
# git 
## git method 
```
git remote show origin
git remote -v
git push origin master
git branch -r
git log --oneline --decorate --graph 
```

git commit -a

# svn

## svn server
svnadmin dump D:\Repositories\iRep > D:\Subversion\svn_all_20200422.dump

## svn ignore
*.gitignore *.user obj bin 

# docker
docker build -t aspnetapp .
docker run -it --rm -p 5000:80 --name aspnetcore_sample aspnetapp
docker run --rm -d aspnetapp
docker ps
docker exec IMAGE_NAMES ipconfig

## dockerfile