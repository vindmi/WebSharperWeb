module Actions

type Action =
    | PolicyView
    | PolicyListView
    | Login of option<Action>
    | Logout

