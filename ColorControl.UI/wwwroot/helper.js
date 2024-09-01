export function FormValid(formId) {
    let form = document.getElementById(formId);
    return form.checkValidity();
}

export function CloseModal(modalId) {
    bootstrap.Modal.getOrCreateInstance('#' + modalId)?.hide();
}

export function OpenModal(modalId) {
    bootstrap.Modal.getOrCreateInstance('#' + modalId)?.show();
}

export function ShowToast(toastId) {
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance('#' + toastId);
    toastBootstrap?.show();
}