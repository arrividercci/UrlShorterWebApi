const urlUri = 'api/url';
const accountUri = 'api/account';
const accessToken = 'accessToken';


function startlogin() {
    document.getElementById('login-model').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
}
function startRegister() {
    document.getElementById('register-model').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
}

document.getElementById('login-button').addEventListener('click', () => startlogin())

document.getElementById('register-button').addEventListener('click', () => startRegister())

function createUrl() {
    const originalUrlTextbox = document.getElementById('original-url').value.trim();
    const customUrlTextbox = document.getElementById('custom-url').value.trim();
    const token = sessionStorage.getItem(accessToken);
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
            if (token !== null) {
                const urlDto = {
                    originalUrl: originalUrlTextbox,
                    shortUrl: customUrlTextbox
                };
                fetch(`${urlUri}/custom`, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
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
}


function login() {
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
    .then(data => loginRes(data))
    .catch(error => console.error("Unable to login", error));
}

function register() {
    const emailTextbox = document.getElementById('register-email').value.trim();
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
    .catch(error => console.error("Unable to login", error));

    document.getElementById('register-model').style.display = 'none';
    document.getElementById('url-model').style.display = 'block';
    
}

function loginRes(data) {
    sessionStorage.setItem(accessToken, data.token);
    document.getElementById('login-button').style.display = 'none';
    document.getElementById('register-button').style.display = 'none';
    document.getElementById('login-model').style.display = 'none';
    document.getElementById('url-model').style.display = 'block';
    document.getElementById('custom-url').style.display = 'block';
    document.getElementById('user-button').style.display = 'block';
    document.getElementById('logout-button').style.display = 'block';
}

async function logout() {
    sessionStorage.removeItem(accessToken);
    document.getElementById('login-button').style.display = 'block';
    document.getElementById('register-button').style.display = 'block';
    document.getElementById('user-button').style.display = 'none';
    document.getElementById('custom-url').style.display = 'none';
    document.getElementById('logout-button').style.display = 'none';
}

document.getElementById('logout-button').addEventListener('click', () => logout())

function displayUrl(urlDto) {
    document.getElementById('original-url').value = urlDto.url;
    document.getElementById('custom-url').value = '';
}
