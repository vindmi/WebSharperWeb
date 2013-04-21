namespace Website

module Actions =
    type Action =
        | Home
        | NewPolicy
        | PolicyList
        | Login of option<Action>
        | Logout
        | Register

    let ProtectedActions =
        [ 
            NewPolicy
            PolicyList 
        ]

    let IsProtected action =
        List.exists (fun a -> a = action) ProtectedActions

