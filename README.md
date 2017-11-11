# Blog

This repo contains the simple blog engine. This is my hobby project. I use it for study asp.net core mvc.

Project combines a asp.net core mvc template with Individual Auth and bootstrap 4 template for blog and post:
https://startbootstrap.com/template-overviews/blog-home/
http://startbootstrap.com/template-overviews/blog-post/


MailGun setup
-------------
For mail sending i use MailGun rest api service. For using you need save domain and api key in user secrets.

```Shell
dotnet user-secrets set MailgunDomain "sandboxxxxxxxxxxxxxxxx.mailgun.org"
dotnet user-secrets set MailgunApiKey "key-xxxxxxxxxxxxxxxxxxxxx"
```


Docker
------
If you need create docker image - use this script

### Attantion
This script stop and remove all containers

```Shell
dotnet restore
dotnet build
dotnet publish -o:./published
docker stop $(docker ps -a -q)
docker rm $(docker ps -a -q)
docker build -t blogapp .
docker run -d -p 80:5000 blogapp
```


Basic usage
-----------
Copy application in server. Run this commands in application folder

```Shell
dotnet restore
dotnet build
dotnet run
```