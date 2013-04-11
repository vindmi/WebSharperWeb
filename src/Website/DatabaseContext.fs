namespace Website

open Model
open System.Data.Entity

    type DatabaseContext() =
        inherit DbContext()

        [<DefaultValue>]
        val mutable users : DbSet<User>
        member this.Users with get() = this.users and set(v) = this.users <- v