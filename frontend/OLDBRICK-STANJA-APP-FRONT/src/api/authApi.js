import axios from "axios";

const API_BASE_URL = "http://localhost:5173";

const TOKEN_KEY = "token";

function getToken() {
    localStorage.getItem(TOKEN_KEY);
}
function setToken(token){
    localStorage.setItem(TOKEN_KEY, token);
}
function removeToken() {
    localStorage.removeItem(TOKEN_KEY);
}

async function login(credentials){
    var response = await axios.post(API_BASE_URL + "/api/auth/login", {
        username: credentials.username,
        password: credentials.password
    });

    var token = response.data.token || (response.data.response && response.data.response.token);

    if (!token) {
    throw new Error("Token is missing in login response");
  }

  setToken(token);
  return response.data;

}

async function registerUser(payload) {
  var response = await axios.post(API_BASE_URL + "/api/users", payload);
  return response.data;
}

export {
  login,
  registerUser,
  getToken,
  setToken,
  removeToken,
};