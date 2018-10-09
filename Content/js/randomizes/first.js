function randomize() {
    const n = Math.floor(Math.random() * 5) + 1;
    let coef = [];
    for (let i = 0; i < n; i++) {
        for (let j = 0; j < n + 1; j++) {
            coef.push(Math.floor((Math.random() * 20 - 10) * 1000) / 1000);
        }
        
        document.getElementsByName('coefficients')[0].value = coef.join(', ');
    }
}