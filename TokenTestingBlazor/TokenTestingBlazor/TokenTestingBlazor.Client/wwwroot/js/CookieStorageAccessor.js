export function get(key) {
    return document.cookie
        .split("; ")
        .find(row => row.startsWith(key + "="))
        ?.split("=")[1];
}

export function set(key, value, domain = "http://localhost:3000") {
    document.cookie = `${key}=${value};domain=${domain};path=/`;
}