import * as firebase from "firebase/app";
import "firebase/auth";
import "firebase/database";
// import withFirebaseAuth from "react-with-firebase-auth";

const firebaseConfig = {
    apiKey: "AIzaSyAYklpbjsUCtEBrS6NVstfwvPYa79cpLbs",
    authDomain: "asic-f94d0.firebaseapp.com",
    databaseURL: "https://asic-f94d0.firebaseio.com",
    projectId: "asic-f94d0",
    storageBucket: "asic-f94d0.appspot.com",
    messagingSenderId: "917363241961",
    appId: "1:917363241961:web:6beec3318799693235d178",
    measurementId: "G-G90TJJ2B3Q"
};


if (!firebase.apps.length) {
    firebase.initializeApp(firebaseConfig);
}


export const auth = firebase.auth();
