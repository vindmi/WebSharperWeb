module Model

open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

    [<Table("User")>]
    type User() =
        [<Key>]
        [<Column("Id")>]
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set

        [<Column("username")>]
        [<Required>]
        member val Username = "" with get, set

        [<Column("first_name")>]
        [<Required>]
        member val FirstName = "" with get, set

        [<Column("last_name")>]
        [<Required>]
        member val LastName = "" with get, set