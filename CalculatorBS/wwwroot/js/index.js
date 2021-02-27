function deleteHistory(id) {
    let element = document.getElementById(id);
    element.innerHTML = '';
    element.remove();
}