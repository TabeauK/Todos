import { Action, Reducer } from 'redux';
import { isNull } from 'util';
import { ApplicationState, AppThunkAction } from '.';

// -----------------
// STATE - This defines the type of data valuestained in the Redux store.

export interface TasksState {
    isLoadingTasks: boolean;
    Tasks: Array<Task>;
    User: User | null;
    lastUpdate: number;
    AllUsers: Array<User>;
}

export interface User {
    userName: string;
    userId: number;
    preferedCalendar: string;
}

export interface Task {
    taskId: number;
    user: User;
    state: State;
    addedDate: Date;
    scheduled: Date | null;
    checks: Array<Check>;
    name: string;
    interval: number;
    extraTime: number;
}

export enum State {
    Safe = 2,
    Scheduled = 1,
    Close = 5,
    VeryClose = 6,
    Urgent = 4
}

export interface Check {
    checkId: number,
    date: Date,
}

export 


// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestTasksAction {
    type: 'REQUEST_TASKS';
    user: User;
}

interface RequestUserAction {
    type: 'REQUEST_USER';
}

interface RequestUsersAction {
    type: 'REQUEST_USERS';
}

interface ReceiveTasksAction {
    type: 'RECEIVE_TASKS';
    tasks: Array<Task>;
}

interface ReceiveUserAction {
    type: 'RECEIVE_USER';
    user: User | null;
}

interface ReceiveUsersAction {
    type: 'RECEIVE_USERS';
    users: Array<User>;
}

interface CheckedTaskAction {
    type: 'CHECKED_TASK';
    task: Task;
    isOk: boolean;
}

interface DeletedTaskAction {
    type: 'DELETED_TASK';
    task: Task;
    isOk: boolean;
}

interface PostponedTaskAction {
    type: 'POSTPONED_TASK';
    task: Task;
    isOk: boolean;
}

interface ScheduledTaskAction {
    type: 'SCHEDULED_TASK';
    task: Task;
}

interface AddedTaskAction {
    type: 'ADDED_TASK';
    task: Task | null;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction =
    CheckedTaskAction |
    DeletedTaskAction |
    PostponedTaskAction |
    ScheduledTaskAction |
    AddedTaskAction |
    ReceiveTasksAction |
    ReceiveUserAction |
    RequestUserAction |
    RequestTasksAction |
    RequestUsersAction |
    ReceiveUsersAction


// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).


const requestTasks = (dispatch: (action: KnownAction) => void, getState: () => ApplicationState) => {
    // Only load data if it's something we don't already have (and are not already loading)
    const appState = getState();
    if (appState && appState.tasks && appState.tasks.User) {
        dispatch({ type: 'REQUEST_TASKS', user: appState.tasks.User });
        fetch(`main/GetTasks/${appState.tasks.User.userId}`)
            .then(response => response.json() as Promise<Task[]>)
            .then(data => {
                dispatch({ type: 'RECEIVE_TASKS', tasks: data });
            });
    }
}

export const actionCreators = {
    requestTasks: (): AppThunkAction<KnownAction> => (dispatch, getState) => { requestTasks(dispatch, getState) },
    requestUsers: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState) {
            dispatch({ type: 'REQUEST_USERS' });
            fetch(`main/GetUsers`)
                .then(response => response.json() as Promise<Array<User>>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_USERS', users: data });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    requestUser: (userId: number | null): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState) {
            dispatch({ type: 'REQUEST_USER' });
            fetch(`main/GetUser/${!!userId ? userId : 0}`)
                .then(response => response.json() as Promise<User>)
                .then(data => {
                    dispatch({ type: 'RECEIVE_USER', user: !!data.userId ? data : null });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    checkTaskAction: (task: Task): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState) {
            fetch(`main/Check/${task.taskId}`)
                .then(response => {
                    dispatch({ type: 'CHECKED_TASK', isOk: response.ok, task: task });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    postponeTaskAction: (task: Task): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState) {
            fetch(`main/Postpone/${task.taskId}`)
                .then(response => {
                    dispatch({ type: 'POSTPONED_TASK', isOk: response.ok, task: task });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    deleteTaskAction: (task: Task): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState) {
            fetch(`main/Delete/${task.taskId}`, { method: 'DELETE' })
                .then(response => {
                    dispatch({ type: 'DELETED_TASK', isOk: response.ok, task: task });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    scheduleTaskAction: (taskId: number, date: Date, timeInterval: Date | null): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                id: taskId,
                date: date,
                meeting: timeInterval,
            })
        };
        // Only load data if it's something we don't already have (and are not already loading)

        if (appState) {
            fetch(`main/Schedule`, requestOptions)
                .then(response => response.json() as Promise<Task>)
                .then(data => {
                    dispatch({ type: 'SCHEDULED_TASK', task: data });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
    addTaskAction: (name: string, interval: number, lastDate: Date | null): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const appState = getState();
        if (appState && appState.tasks && appState.tasks.User) {
            const requestOptions = {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    userId: appState.tasks.User.userId,
                    name: name,
                    lastDate: lastDate,
                    interval: interval
                })
            };
            fetch(`main/Create`, requestOptions)
                .then(response => response.json() as Promise<Task>)
                .then(data => {
                    dispatch({ type: 'ADDED_TASK', task: data });
                }).then(() => requestTasks(dispatch, getState));
        }
    },
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: TasksState = {
    isLoadingTasks: true,
    Tasks: [],
    User: null,
    lastUpdate: Date.now(),
    AllUsers: new Array<User>(),
};

export const reducer: Reducer<TasksState> = (state: TasksState | undefined, incomingAction: Action): TasksState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'ADDED_TASK':
            if (!isNull(action.task)) {
                let tasks = state.Tasks;
                tasks.push(action.task as Task);
                return {
                    ...state,
                    Tasks: tasks,
                };
            }
            break;
        case 'SCHEDULED_TASK':
            if (!!action.task) {
                let tasks = state.Tasks
                tasks[tasks.findIndex(x => action.task.taskId === x.taskId)] = action.task;
                return {
                    ...state,
                    Tasks: tasks,
                };
            }
            break;
        case 'DELETED_TASK':
            if (action.isOk) {
                let tasks = state.Tasks
                tasks.splice(tasks.findIndex(x => action.task.taskId === x.taskId), 1);
                return {
                    ...state,
                    Tasks: tasks,
                };
            }
            break;
        case 'POSTPONED_TASK':
            if (action.isOk) {
                let tasks = state.Tasks
                tasks[tasks.findIndex(x => action.task.taskId === x.taskId)] = action.task;
                return {
                    ...state,
                    Tasks: tasks,
                };
            }
            break;
        case 'CHECKED_TASK':
            if (action.isOk) {
                let tasks = state.Tasks
                tasks[tasks.findIndex(x => action.task.taskId === x.taskId)] = action.task;
                return {
                    ...state,
                    Tasks: tasks,
                };
            }
            break;
        case 'RECEIVE_USER':
            return {
                ...state,
                User: (action as ReceiveUserAction).user,
            };
        case 'RECEIVE_USERS':
            return {
                ...state,
                AllUsers: (action as ReceiveUsersAction).users,
            };
        case 'RECEIVE_TASKS':
            return {
                ...state,
                isLoadingTasks: false,
                Tasks: (action as ReceiveTasksAction).tasks,
                lastUpdate: Date.now(),
            };
        case 'REQUEST_USER':
            return state;
        case 'REQUEST_USERS':
            return state;
        case 'REQUEST_TASKS':
            return {
               ...state,
                isLoadingTasks: true,
            };
    }
    return state;
};
