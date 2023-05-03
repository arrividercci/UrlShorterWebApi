const urlUri = 'api/url';
const accountUri = 'api/account';
const accessToken = 'accessToken';


function startlogin() {
    document.getElementById('login-model').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
}

document.getElementById('login-button').addEventListener('click', () => startlogin())

async function createUrl() {
    const originalUrlTextbox = document.getElementById('original-url').value.trim();
    const customUrlTextbox = document.getElementById('custom-url').value.trim();
    if (originalUrlTextbox != '') {
        if (customUrlTextbox == '') {
            const urlDto = {
                url: originalUrlTextbox
            };
            fetch(urlUri, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(urlDto)
            })
            .then(response => response.json())
            .then(url => displayUrl(url))
                .catch(error => console.error("Unable to create", error));
        } else {
            const token = sessionStorage.getItem(accessToken);
            const urlDto = {
                url: originalUrlTextbox,
                customUrl: customUrlTextbox
            };
            fetch(`${urlUri}\custom`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Authorization': 'Bearer ' + token
                },
                body: JSON.stringify(urlDto)
            })
            .then(response => response.json())
            .then(url => displayUrl(url))
            .catch(error => console.error("Unable to create", error));
        }
    }
}


async function login() {
    const emailTextbox = document.getElementById('login-email').value.trim();
    const passwordTextbox = document.getElementById('login-password').value.trim();
    const loginUser = {
        email: emailTextbox,
        password: passwordTextbox
    };
    fetch(`${accountUri}/login`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginUser)
    })
    .then(response => response.json())
    .then(response => response.json)
    .then(data => loginRes(data))
    .catch(error => console.error("Unable to login", error));
}

function loginRes(data) {
    sessionStorage.setItem(accessToken, data.token);
    document.getElementById('login-button').style.display = 'none';
    document.getElementById('register-button').style.display = 'none';
    document.getElementById('login-model').style.display = 'none';
    document.getElementById('url-model').style.display = 'block';
    document.getElementById('user-button').style.display = 'block';
    document.getElementById('logout-button').style.display = 'block';
}

async function logout() {
    sessionStorage.removeItem(accessToken);
    document.getElementById('login-button').style.display = 'block';
    document.getElementById('register-button').style.display = 'block';
    document.getElementById('user-button').style.display = 'none';
    document.getElementById('logout-button').style.display = 'none';
}

async function register() {
    const emailTextbox = document.getElementByIdll('register-email').value.trim();
    const passwordTextbox = document.getElementById('register-password').value.trim();
    const confirmPasswordTextbox = document.getElementById('register-confirm-password').value.trim();
    const registerUser = {
        email: emailTextbox,
        password: passwordTextbox,
        confirmPassword: confirmPasswordTextbox
    };
    fetch(`${accountUri}/register`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(registerUser)
    })
    .then(response => response.json())
    .then(async (response) => {
        if (response.ok === true) {
        } else {
                
        }
    })
    .catch(error => console.error("Unable to register", error));

}

function displayUrl(urlDto) {
    document.getElementById('original-url').value = urlDto.url;
}
