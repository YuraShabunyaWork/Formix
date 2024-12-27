document.getElementById('create-button').addEventListener('click', function (e) {
    e.preventDefault();

    const container = document.getElementById('questions-container');
    const questionsItem = container.getElementsByClassName('question-item');

    for (let i = 0; i < questionsItem.length; i++) {
        const tupeQuestions = questionsItem[i].getElementsByClassName('type-question');
        let input = tupeQuestions[0];
        input.name = 'Questions[' + i + '].TypeQuestion';

        const questions = questionsItem[i].getElementsByClassName('question');
        input = questions[0];
        input.name = 'Questions[' + i + '].Title';

        const optionsQuestions = questionsItem[i].getElementsByClassName('options-question');
        for (let j = 0; j < optionsQuestions.length; j++) {
            input = optionsQuestions[j];
            input.name = 'Questions[' + i + '].OptionsAnswer[' + j + ']';
        }
    }
    e.target.closest('form').submit();
});
