import { auth } from "./firebase";
import * as firebase from "firebase/app";


export const doSignInWithGooogle = () => {
    const googleProvider = new firebase.auth.GoogleAuthProvider();
    return auth.signInWithPopup(googleProvider);
}

export const doSignOut = () => auth.signOut();

export const onAuthStateChanged = (nextOrObserver:
    | firebase.Observer<any>
    | ((a: firebase.User | null) => any),
    error?: (a: firebase.auth.Error) => any,
    completed?: firebase.Unsubscribe) => {
    auth.onAuthStateChanged(nextOrObserver, error, completed);
};
