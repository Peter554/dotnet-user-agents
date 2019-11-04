# dotnet-user-agents

Live @ https://peter554--dotnet-user-agents.herokuapp.com

```
heroku container:push web --app peter554--dotnet-user-agents

heroku container:release web --app peter554--dotnet-user-agents
```

---

- https://docs.docker.com/engine/examples/dotnetcore/
- https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/Dockerfile
- https://devcenter.heroku.com/articles/container-registry-and-runtime

---

Local Docker:

```
docker build -t peter554/dotnet-user-agents .

docker run -d -p 8080:80 --name dotnetapp peter554/dotnet-user-agents
```