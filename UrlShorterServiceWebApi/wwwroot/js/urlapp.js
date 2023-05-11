const urlUri = 'api/url';
const accountUri = 'api/account';
const accessToken = 'accessToken';

let userUrls = [];

let id = null;

let form = document.querySelector('.form-wrapper');
let tbody = document.querySelector('tbody');
let save = document.querySelector('.save');
let cancel = document.querySelector('.cancel');

cancel.onclick = function() {
    form.classList.remove('active');
}

function ushorter() {
    document.getElementById('login-model').style.display = 'none';
    document.getElementById('url-model').style.display = 'block';
    document.getElementById('register-model').style.display = 'none';
    document.getElementById('url-table').style.display = 'none';
}

function startlogin() {
    document.getElementById('login-model').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
    document.getElementById('register-model').style.display = 'none';
}
function startRegister() {
    document.getElementById('register-model').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
    document.getElementById('login-model').style.display = 'none';
}


document.getElementById('user-button').addEventListener('click', () => startHome())

document.getElementById('login-button').addEventListener('click', () => startlogin())

document.getElementById('register-button').addEventListener('click', () => startRegister())

function createUrl() {
    const originalUrlTextbox = document.getElementById('original-url').value.trim();
    const customUrlTextbox = document.getElementById('custom-url').value.trim();
    const token = sessionStorage.getItem(accessToken);
    if (originalUrlTextbox != '') {
        if (sessionStorage.getItem(accessToken) == null) {
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
            .catch(error => console.error('Unable to create', error));
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
                .catch(error => console.error('Unable to create', error));
            }
        }
    }
}

function editUrl(event) {
    form.classList.add('active');
    id = event.target.parentElement.parentElement.id;
}

function deleteUrl(e) {
    const token = sessionStorage.getItem(accessToken);
    id = event.target.parentElement.parentElement.id;
    fetch(`${urlUri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + token
        }
    })
    .catch(error => console.error('Cannot get urls', error));

    getUserUrls();

    form.classList.remove('active');
}


function updateTable() {
    let data = "";
    for (i = 0; i < userUrls.length; i++) {
        data += `<tr id="${userUrls[i]['id']}">
                    <td>${userUrls[i]['originalUrl']}<td>
                    <td>${userUrls[i]['shortUrl']}<td>
                    <td><button class="btn" onclick="editUrl(event)">Edit</button><td>
                    <td><button class="btn" onclick="deleteUrl(event)">Delete</button><td>
                 </tr>
        `
    }
    tbody.innerHTML = data;
}

function getUserUrls() {
    const token = sessionStorage.getItem(accessToken);
    fetch(`${urlUri}/my`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    })
    .then(response => response.json())
    .then(urls => displayUrls(urls))
    .catch(error => console.error('Cannot get urls', error));
}

function startHome() {
    document.getElementById('url-table').style.display = 'block';
    document.getElementById('url-model').style.display = 'none';
    getUserUrls();
}

save.onclick = function () {
    const shortUrlText = document.getElementById('url').value.trim();
    const token = sessionStorage.getItem(accessToken);
    const editUrl = {
        url: shortUrlText
    };
    fetch(`${urlUri}/${id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(editUrl)
    })
    .catch(error => console.error('Cannot get urls', error));

    getUserUrls();

    form.classList.remove('active');
}

function displayUrls(urls) {
    userUrls = urls;
    updateTable();
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
    .catch(error => console.error('Unable to login', error));
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
    .catch(error => console.error('Unable to register', error));

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
    getUserUrls();
}

async function logout() {
    sessionStorage.removeItem(accessToken);
    document.getElementById('login-button').style.display = 'block';
    document.getElementById('register-button').style.display = 'block';
    document.getElementById('user-button').style.display = 'none';
    document.getElementById('custom-url').style.display = 'none';
    document.getElementById('logout-button').style.display = 'none';
    document.getElementById('url-table').style.display = 'none';
    document.getElementById('url-model').style.display = 'block';
}

document.getElementById('logout-button').addEventListener('click', () => logout())

function displayUrl(urlDto) {
    document.getElementById('original-url').value = urlDto.url;
    document.getElementById('custom-url').value = '';
}

