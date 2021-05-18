var GALLERY = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var fullGalleryMosaic = [
        { w: 2, h: 2, c: 1, r: 1 },
        { w: 1, h: 1, c: 3, r: 1 },
        { w: 1, h: 1, c: 3, r: 2 },
        { w: 1, h: 2, c: 4, r: 1 },
        { w: 2, h: 1, c: 5, r: 1 },
        { w: 2, h: 2, c: 5, r: 2 },
        { w: 1, h: 1, c: 4, r: 3 },
        { w: 2, h: 1, c: 2, r: 3 },
        { w: 1, h: 2, c: 1, r: 3 },
        { w: 1, h: 1, c: 2, r: 4 },
        { w: 2, h: 1, c: 3, r: 4 },
        { w: 1, h: 1, c: 5, r: 4 },
        { w: 1, h: 1, c: 6, r: 4 }
    ];

    var fullGalleryMosaicXS = [
        { w: 2, h: 2, c: 1, r: 1 },
        { w: 1, h: 1, c: 3, r: 1 },
        { w: 1, h: 1, c: 3, r: 2 },
        { w: 1, h: 2, c: 1, r: 3 },
        { w: 2, h: 1, c: 2, r: 3 },
        { w: 1, h: 1, c: 2, r: 4 },
        { w: 1, h: 1, c: 3, r: 4 }
    ];

    var fullGalleryMosaicSM = [
        { w: 2, h: 2, c: 1, r: 1 },
        { w: 1, h: 1, c: 3, r: 1 },
        { w: 1, h: 1, c: 3, r: 2 },
        { w: 1, h: 2, c: 1, r: 3 },
        { w: 2, h: 1, c: 2, r: 3 },
        { w: 1, h: 1, c: 2, r: 4 },
        { w: 1, h: 1, c: 3, r: 4 }
    ];

    function init() {
        setSelectors();
        cacheObjects();
    }

    function setSelectors() {
        selectors.xxx = '#xxx';
    }

    function cacheObjects() {
        objs.xxx = $(selectors.xxx);
    }

    function load(selector, type) {
        if (!type) {
            loadGalleryOne(selector);
        }
        else if (type === 2) {
            loadGalleryTwo(selector);
        }
    }

    function loadGalleryOne(selector) {
        var galleryElement = $(selector);

        var nanoGalleryJson = galleryElement.data('items');

        galleryElement.nanogallery2({
            // CONTENT SOURCE
            items: nanoGalleryJson,

            // GALLERY AND THUMBNAIL LAYOUT
            galleryMosaic: fullGalleryMosaic,
            galleryMosaicXS: fullGalleryMosaicXS,
            galleryMosaicSM: fullGalleryMosaicSM,

            galleryMaxRows: 1,
            galleryDisplayMode: 'fullContent',
            gallerySorting: 'random',
            thumbnailDisplayOrder: 'random',

            thumbnailHeight: '180', thumbnailWidth: '220',
            thumbnailAlignment: 'center',
            thumbnailGutterWidth: 0, thumbnailGutterHeight: 0,
            thumbnailBorderHorizontal: 0, thumbnailBorderVertical: 0,

            thumbnailToolbarImage: null,
            thumbnailToolbarAlbum: null,
            thumbnailLabel: { display: false },

            // DISPLAY ANIMATION
            // for gallery
            galleryDisplayTransitionDuration: 1500,
            // for thumbnails
            thumbnailDisplayTransition: 'imageSlideUp',
            thumbnailDisplayTransitionDuration: 1200,
            thumbnailDisplayTransitionEasing: 'easeInOutQuint',
            thumbnailDisplayInterval: 60,

            // THUMBNAIL HOVER ANIMATION
            thumbnailBuildInit2: 'image_scale_1.15',
            thumbnailHoverEffect2: 'thumbnail_scale_1.00_1.05_300|image_scale_1.15_1.00',
            touchAnimation: true,
            touchAutoOpenDelay: 500,

            // GALLERY THEME
            galleryTheme: {
                thumbnail: { background: '#dedada' },
            },

            // DEEP LINKING
            locationHash: true
        });
    }

    function loadGalleryTwo(selector) {
        console.log('loadGalleryTwo');
        var galleryElement = $(selector);

        var nanoGalleryJson = galleryElement.data('items');

        galleryElement.nanogallery2({
            // CONTENT SOURCE
            items: nanoGalleryJson,

            galleryDisplayMode: 'fullContent',
            thumbnailDisplayOrder: 'random',

            thumbnailHeight: '150',
            thumbnailWidth: 'auto',
            thumbnailAlignment: 'justified',

            thumbnailToolbarImage: null,
            thumbnailToolbarAlbum: null,
            thumbnailLabel: { display: false },

            // DISPLAY ANIMATION
            // for gallery
            galleryDisplayTransitionDuration: 1500,
            // for thumbnails
            thumbnailDisplayTransition: 'scaleUp',
            thumbnailDisplayTransitionDuration: 1200,
            thumbnailDisplayTransitionEasing: 'easeInOutQuint',
            thumbnailDisplayInterval: 60,

            // THUMBNAIL HOVER ANIMATION
            thumbnailBuildInit2: 'image_scale_1.15',
            thumbnailHoverEffect2: 'thumbnail_scale_1.00_1.05_300|image_scale_1.15_1.00',
            touchAnimation: true,
            touchAutoOpenDelay: 500,

            // GALLERY THEME
            galleryTheme: {
                thumbnail: { background: '#fff' },
            },

            // DEEP LINKING
            locationHash: true
        });
    }

    return {
        Init: init,
        Load: load
    };
}());

GALLERY.Init();