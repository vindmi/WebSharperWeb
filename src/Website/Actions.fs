module Actions

type Action =
    | Home
    | NewPolicy
    | PolicyList
    | Login of option<Action>
    | Logout

let ProtectedActions =
    [ 
        NewPolicy
        PolicyList 
    ]

let IsProtected action =
    List.exists (fun a -> a = action) ProtectedActions

