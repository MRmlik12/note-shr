export function openFilePicker() {
    let url = null;
    const input = document.createElement('input');
    input.type = 'file';
    
    input.click();

    while (url == null) {
        if (input.files.length > 0) {
            url = URL.createObjectURL(input.files[0]);
            break;
        }
    }
    
    return url;
}