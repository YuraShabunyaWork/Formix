$(document).on('click', '.add-option', function () {
    const newOption = `
                <li>
                    <div class="d-flex mt-1">
                        <input type="text" class="form-control me-1 options-question" />
                        <button class="btn btn-danger btn-sm remove-option-btn" tabindex="-1" type="button">
                             <i class="bi bi-x-circle"></i>
                        </button>
                    </div>
                </li>
            `;
    $(this).siblings('ul').append(newOption);
});

$(document).on('click', `.remove-option-btn`, function () {
    $(this).closest('li').remove();
});