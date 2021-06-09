﻿var WYSIWYGEDITOR = (function () {
    "use strict";

    var editors = [];

    function bindEditor(selector) {
        var element = document.querySelector(selector);

        return ClassicEditor
            .create(element, {
                ckfinder: {
                    uploadUrl: '/storage/uploadarticleimage'
                },
                toolbar: {
                    items: [
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
                        'blockQuote',
                        'insertTable',
                        'todoList',
                        'imageInsert',
                        'mediaEmbed',
                        'code',
                        'codeBlock',
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
                    ],
                    styles: [],
                },
                table: {
                    contentToolbar: [
                        'tableColumn',
                        'tableRow'
                    ]
                },
                licenseKey: '',
            })
            .then(editor => {
                var existing = editors.filter((e) => {
                    return e.id === editor.id;
                });

                if (existing.length === 0) {
                    editors.push({ id: editor.sourceElement.id, editor: editor });
                }

                return editor.id;
            })
            .catch(error => {
                console.error('Oops, something went wrong!');
                console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
                console.warn('Build id: vjiyoesm40zi-u0d9ila63bix');
                console.error(error);
            });
    }

    function updateSourceElement(editorId) {
        var existing = editors.filter((editor) => {
            return editor.id === editorId;
        });

        if (existing) {
            existing[0].editor.updateSourceElement();
        }
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

    return {
        Editors: editors,
        GetEditor: getEditor,
        DestroyEditor: destroyEditor,
        BindEditor: bindEditor,
        UpdateSourceElement: updateSourceElement
    };
}());