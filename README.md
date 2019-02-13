# SqlHelper

**SqlHelper** is SimpleAdo's lightweight ADO.NET wrapper for accessing the Sql Server database. Its purpose is to:

1. reduce setup repetition, 
2. improve usage intuitiveness, 
3. and clarify functional intent.

A main goal for this project is to provide an easy starting point for users to extend, 
by adhering to SOLID priciples and maintaining clear separation of concerns.

**Example of a Typical SQL Read Using SimpleAdo**

```

string cmdText = “SELECT * FROM Product WHERE Id = @Id”;

var product = new SqlHelper(cmdText).AddParam(“Id”, 1).ExecuteReader<Product>();

              |___________________| |________________| |____________________|
                        |                   |                    |
                 Initialization      Sql Parameters       Execute Method
                                       (optional)                                       

```

This execute method expression becomes more functional in nature by encapsulating its `SqlDataReader` using statement block. 
Since the data returning from the database is set directly to an entity variable within the method, coding intention becomes 
easier to read and cleaner to write.

___

## 1. Initialization

SqlHelper reduces parameter clutter by leveraging common defaults. The simplest constructor overload requires only the command text.

```

public SqlHelper(string commandText)
public SqlHelper(string commandText, string connectionString)
public SqlHelper(string commandText, CommandType commandType)
public SqlHelper(string commandText, CommandType commandType, string connectionString)

```


**Default Connection String**

The SqlHelper constructor makes use of a separate `DefaultConnectionString` static class to retrieve a default 
connection string named `Default` from either the **app.config** or **web.config**. This default name can be 
overwritten at startup using the `SetName(string name)` method.

The connection string's application name may also be set at startup by using the `SetApplicationName(string applicationName)` method.


---


## 2. Sql Parameters

Sql parameters are chainable though a fluent API method named **AddParam** having three overloads that accepts parameters for: 

1. key / value pair
2. SqlParameter object
3. IEnumerable of SqlParameters

**Example Using a SqlParameter Array**

```
string cmdText = “UPDATE Product SET Name = @Name WHERE Id = @Id”;

var param = new SqlParameter[]
{
	new SqlParameter(“Id”, 1),	
    new SqlParameter(“Name”, “New Name”),
};

SqlHelper(cmdText).AddParam(param).ExecuteNonQuery();
```
---

## 3. Execute Commands

**ExecuteNonQuery** returns the number of affected rows as an `int`.

```
string cmdText = “UPDATE Product SET Name = @Name WHERE Id = @Id”;

int rowsUpdated = new SqlHelper(cmdText)
                   .AddParam(“Id”, 1)
                   .AddParam(“Name”, "New Name")
                   .ExecuteNonQuery();
```

**ExecuteScalar** returns an `object` which may then be safely casted using extension methods in SimpleAdo's **DbSafeConverterExtensions** Helper class. 

```
string cmdText = “SELECT MAX(Price) FROM Product”;

decimal price = new SqlHelper(cmdText).ExecuteScalar().ToDecimal();
```

**ExecuteReader** takes any method that accepts `SqlDataReader` as a parameter to return either an instance of this method's return type 
or `null` in the case of an empty result.

```
string cmdText = “SELECT TOP 1 * FROM Product”;

var product = new SqlHelper(cmdText).ExecuteReader(r => Load(r));

...

private Product Load(SqlDataReader reader)
{
   var product = new Product();
   
      // implementation here
   
   return product;
}
```

**ExecuteReader** also provides a parameterless overload for using a type of `T` that implements the **ISqlLoader** interface.

```
interface ISqlLoader
{
    Load(SqlDataReader reader);
}
```

**ExecuteReaderList** returns a `List<T>`. It will return an empty list instead of null if the result set is empty.

```
string cmdText = “SELECT * FROM Product”;

var products = new SqlHelper(cmdText).ExecuteReaderList<Product>();
```


**\*Each execute method has an analogous async version.**

---

## Integration and Unit Testing

A simple database project is included with this solution to be used for integration tests in the **SimpleAdo.IntegrationTests** project.

**SimpleAdo.UnitTests** project provides unit test coverage.


---

## Challenges

The biggest challenge in creating this wrapper is in handling collections since it's problematic to access 
the individual types inside a type that is a generic itself. Ideally I would have preferred the generic collection to 
be handle by the same method signature while allowing for different generic collection types. 

Handling a generic of type collection that implement `ISqlLoader` individually is achievable using reflection to dig 
into the generic list. However, we lose out on the compile time checking of the actually inner class since it's not possible 
to constraint both the generic list and the type that makes up that list. I played around with writing the execute code below. 
It works but I wasn't happy with it from a usage standpoint.

```
private T MultiRowExecuteReader<T>(SqlCommand cmd, Func<SqlDataReader, T> loader) where T : IList, new()
{
    var list = new T();

    using (var reader = cmd.ExecuteReader())
    {
        var itemType = list.GetType().GetGenericArguments()[0];
        var instance = Activator.CreateInstance(itemType) as ISqlLoader;

        if (instance == null)
            throw new ArgumentException(string.Format("The class {0} does not implement ISqlLoader", itemType));

        while (reader.Read())
            list.Add(instance.Load(reader));
    }

    return list;
}

```

The second issue with handling a generic of type collections is writing a method that accepts an external loader. 
Since the loader should operate on the type the collection contains instead of the collection itself, we need to know about that inner
type. But we also need to declare the return value's collection type. Therefore, we are required to use 
both generic types in our declaration, TResult for the resulting collection and TItem for processing the reader.
Even if it's fairly simple to write, this break in our usage consistency is just ugly.

I ultimately made the decision to use a seperate method for collections and load the entities directly into a List<T> for simplicity. 
We lose some flexibility but returning a List<T> is mostly how we would be using it anyways. If we need to instantiate a collection
other than List<T>, we can always use the the more general `Execute` method to and write the execute logic in the lambda code block.

---

## TODO And Improvements

1. Add `CancellationToken` parameter for the async commands.
2. Add support for DataSets.
3. Add other types of ADO.NET access implementations.
4. Improve convention for retrieval of generic collections.
5. Handle the case for unit of work