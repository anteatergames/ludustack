var WYSIWYGEDITOR = (function () {
    "use strict";

    var editors = [];

    function bindEditor(selector) {
        var element = document.querySelector(selector);

        return bindEditorElement(element);
    }

    function bindEditorElement(element) {
        return ClassicEditor
            .create(element, {
                simpleUpload: {
                    uploadUrl: '/storage/uploadarticleimage'
                },
                toolbar: {
                    items: [
                        'heading',
                        '|',
                        'fontColor',
                        'bold',
                        'italic',
                        'link',
                        'bulletedList',
                        'numberedList',
                        '|',
                        'outdent',
                        'indent',
                        'alignment',
                        '|',
                        'imageInsert',
                        'blockQuote',
                        'insertTable',
                        'mediaEmbed',
                        '|',
                        'code',
                        'codeBlock',
                        '|',
                        'horizontalLine',
                        'specialCharacters',
                        '|',
                        'strikethrough',
                        'subscript',
                        'superscript',
                        'underline',
                        '|',
                        'undo',
                        'redo'
                    ]
                },
                mediaEmbed: {
                    previewsInData: true
                },
                language: 'en',
                image: {
                    toolbar: [
                        'imageTextAlternative',
                        'imageStyle:inline',
                        'imageStyle:block',
                        'imageStyle:side',
                        'linkImage'
                    ]
                },
                table: {
                    contentToolbar: [
                        'tableColumn',
                        'tableRow',
                        'mergeTableCells'
                    ]
                },
                licenseKey: '',
            })
            .then(editor => {
                var existing = editors.filter((e) => {
                    return e.id === editor.id;
                });

                if (existing.length === 0) {
                    editors.push({ id: editor.id, editor: editor });
                }

                element.setAttribute('data-editor-id', editor.id);

                return editor.id;
            })
            .catch(error => {
                console.error('Oops, something went wrong!');
                console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
                console.warn('Build id: vjiyoesm40zi-u0d9ila63bix');
                console.error(error);
            });
    }

    function bindEditors(selector) {
        var elements = document.querySelectorAll(selector);

        for (const element of elements) {
            bindEditorElement(element);
        }
    }

    function updateSourceElement(editorId) {
        var existing = editors.filter((editor) => {
            return editor.id === editorId;
        });

        if (existing) {
            existing[0].editor.updateSourceElement();
        }
    }

    function updateEditors() {
        $('.wysiwygeditor').each((index, element) => {
            var editorId = $(element).attr('data-editor-id');
            WYSIWYGEDITOR.UpdateSourceElement(editorId);
        });
    }

    function destroyEditor(editorId) {
        var existing = editors.filter((editor) => {
            return editor.id === editorId;
        });

        if (existing.length === 1) {
            var index = editors.indexOf(existing[0]);

            existing[0].editor.destroy()
                .catch(error => {
                    console.log(error);
                });

            editors.splice(index);
        }
    }

    function getEditor(editorId) {
        var existing = editors.filter((editor) => {
            return editor.id === editorId;
        });

        if (existing) {
            return existing[0];
        }
        else {
            return null;
        }
    }

    function setValidatorDefaults() {
        $.validator.setDefaults({
            ignore: ":hidden:not(.wysiwygeditor)"
        });
    }

    return {
        Editors: editors,
        GetEditor: getEditor,
        DestroyEditor: destroyEditor,
        BindEditor: bindEditor,
        BindEditorElement: bindEditorElement,
        BindEditors: bindEditors,
        UpdateSourceElement: updateSourceElement,
        UpdateEditors: updateEditors,
        SetValidatorDefaults: setValidatorDefaults
    };
}());