FROM microsoft/aspnetcore
WORKDIR /app
COPY ./published .
ENTRYPOINT ["dotnet", "Blog.dll"]