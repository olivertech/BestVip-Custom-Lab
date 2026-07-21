document.documentElement.classList.add("js-ready");

document.querySelectorAll('[data-mask="whatsapp"]').forEach((input) => {
    const applyMask = () => {
        const digits = input.value.replace(/\D/g, "").slice(0, 11);

        if (digits.length <= 2) {
            input.value = digits.length ? `(${digits}` : "";
            return;
        }

        if (digits.length <= 7) {
            input.value = `(${digits.slice(0, 2)}) ${digits.slice(2)}`;
            return;
        }

        if (digits.length <= 10) {
            input.value = `(${digits.slice(0, 2)}) ${digits.slice(2, 6)}-${digits.slice(6)}`;
            return;
        }

        input.value = `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7)}`;
    };

    input.addEventListener("input", applyMask);
    input.addEventListener("blur", applyMask);
});
