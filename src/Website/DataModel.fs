namespace Website.DataModel
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

    [<Table("Policy")>]
    type Policy() =
        [<Key>]
        [<Column("id")>]
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set

    [<Table("InsuredObject")>]
    type InsuredObject() =
        [<Key>]
        [<Column("policy_id")>]
        member val PolicyId = 0 with get, set

    [<Table("Client")>]
    type Client() =
        [<Key>]
        [<Column("id")>]
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set

        [<Column("login")>]
        [<Required>]
        member val Login = "" with get, set

        [<Column("code")>]
        member val Code = "00000000000" with get, set
        
        [<Column("first_name")>]
        [<Required>]
        member val FirstName = "" with get, set

        [<Column("last_name")>]
        [<Required>]
        member val LastName = "" with get, set

        [<Column("birth_date")>]
        [<Required>]
        member val BirthDate = System.DateTime.Now with get, set