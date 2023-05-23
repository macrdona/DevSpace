# Wrappers

## Definition

- A wrapper class is any class which "wraps" or encapsulates the functionality of another class or component. These are useful by providing a level of abstraction from the implementation of the underlying class or component.

- Through a wrapper class we are able to select what features we want to expose to the overall program.

- These are also useful when it comes to Unit Testing. When unit testing, we usually need to create fakes/mocks to test a function. However, there are classes that rely on static/extention methods which are difficult to mock due to their complexity. In those cases, we can create wrapper classes which allows us to encapsulate the functionality of the static method in our own defined methods, which we can then utilize for testing purposes.

## Example

```
public interface IBCryptWrapper
{
    string HashPassword(string inputKey);
}

public class BCryptWrapper : IBCryptWrapper
{

    public string HashPassword(string inputKey)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(inputKey);
        return hash;
    }
}
```

- HashPassword() is a static method from the BCrypt library.
- We implement a wrapper class (note the method is no longer static).
- Inside this new method we call the original static function to perform the neccesary operation and return the results.
