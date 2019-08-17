# Elegant400
a custom BadRequest format for ASP.NET Core, for creating the following structure in validation responses:
```
{
  title = "Validation", 
  errors = [
    { error = "required", path = ["people", 1, "surname"] },
    { error = "minLength", path = ["summary"], length = 3 }
  ]
}
```
