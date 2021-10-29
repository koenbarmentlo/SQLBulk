# SQLBulk

```C#
[Table("CoolTableName")]//if omitted, class name is used
public class Person
{
    [PrimaryKey(true)] //true if primary key is auto generated, using on multiple properties is allowed.
    public int Id { get; set; }
    [Column("SomeCoolColumnName")]//maps property to column name. if omitted, property name is used
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Ignore]
    public int Age { get; set; }
}

public class Docs
{
    public void Example()
    {
        using var connection = new SqlConnection("Connection string here");
        connection.Open();

        connection.BulkInsert(new Person[]
        {
            new Person()
            {
                Age = 10,
                FirstName = "Hans",
                LastName = "Worst"
            }
        });

        connection.BulkUpdate(new Person[]
        {
            new Person()
            {
                Id = 1,
                Age = 10,
                FirstName = "Hans",
                LastName = "Worst"
            }
        }, new Options.UpdateBulkOptions()
        {
            MatchOnColumnNames = new string[] { nameof(Person.Id) }
        });

        connection.BulkInsertOrUpdate(new Person[]
        {
            new Person()
            {
                Id = 1,
                Age = 10,
                FirstName = "Hans",
                LastName = "Worst"
            }
        }, new Options.UpdateBulkOptions()
        {
            MatchOnColumnNames = new string[] { nameof(Person.Id) }
        });

        connection.BulkInsertOrUpdateOrDelete(new Person[]
        {
            new Person()
            {
                Id = 1,
                Age = 10,
                FirstName = "Hans",
                LastName = "Worst"
            }
        }, new Options.UpdateBulkOptions()
        {
            MatchOnColumnNames = new string[] { nameof(Person.Id) }
        });

        //All these method have an async variant each and work the same
    }
}
```

- For more info on options see comments on properties.
- Tested on LocalDB 15
- SQL Server only
