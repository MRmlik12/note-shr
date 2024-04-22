export const openFilePicker = async () => {
    const input = document.createElement('input');
    
    input.type = 'file';
    input.onchange = _ => {
        console.log(input.files);
    };
    
    input.click();
}