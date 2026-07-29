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

document.querySelectorAll("[data-user-menu]").forEach((menu) => {
    const trigger = menu.querySelector("[data-user-menu-trigger]");
    const modal = menu.querySelector("[data-user-menu-modal]");

    if (!trigger || !modal) {
        return;
    }

    const closeMenu = () => {
        modal.hidden = true;
        trigger.setAttribute("aria-expanded", "false");
    };

    trigger.addEventListener("click", (event) => {
        event.stopPropagation();
        const willOpen = modal.hidden;

        document.querySelectorAll("[data-user-menu-modal]").forEach((otherModal) => {
            otherModal.hidden = true;
        });

        document.querySelectorAll("[data-user-menu-trigger]").forEach((otherTrigger) => {
            otherTrigger.setAttribute("aria-expanded", "false");
        });

        modal.hidden = !willOpen;
        trigger.setAttribute("aria-expanded", willOpen ? "true" : "false");
    });

    document.addEventListener("click", (event) => {
        if (!menu.contains(event.target)) {
            closeMenu();
        }
    });
});

document.querySelectorAll("[data-survey-confirm-form]").forEach((form) => {
    const modal = form.querySelector("[data-survey-confirm-modal]");
    const acceptButton = form.querySelector("[data-survey-confirm-accept]");
    const cancelButtons = form.querySelectorAll("[data-survey-confirm-cancel]");

    if (!modal || !acceptButton || cancelButtons.length === 0) {
        return;
    }

    let isConfirmedSubmit = false;

    const closeModal = () => {
        modal.hidden = true;
        document.body.classList.remove("modal-open");
    };

    const openModal = () => {
        modal.hidden = false;
        document.body.classList.add("modal-open");
    };

    form.addEventListener("submit", (event) => {
        if (isConfirmedSubmit) {
            isConfirmedSubmit = false;
            return;
        }

        event.preventDefault();
        openModal();
    });

    acceptButton.addEventListener("click", () => {
        isConfirmedSubmit = true;
        closeModal();
        form.requestSubmit();
    });

    cancelButtons.forEach((button) => {
        button.addEventListener("click", closeModal);
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && !modal.hidden) {
            closeModal();
        }
    });
});
