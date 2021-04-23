var COSTCALCULATOR = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var rates;
    var selectedArtStyle;
    var selectedSoundStyle;

    var conceptCount;
    var characterCount;
    var levelCount;
    var soundFxCount;
    var musicCount;

    var minimumConceptCost = 0;
    var minimumCharacterCost = 0;
    var minimumLevelCost = 0;
    var minimumSoundFxCost = 0;
    var minimumMusicCost = 0;

    var averageConceptCost = 0;
    var averageCharacterCost = 0;
    var averageLevelCost = 0;
    var averageSoundFxCost = 0;
    var averageMusicCost = 0;

    var maximumConceptCost = 0;
    var maximumCharacterCost = 0;
    var maximumLevelCost = 0;
    var maximumSoundFxCost = 0;
    var maximumMusicCost = 0;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.canInteract = '#caninteract';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmRateCalculator';
        selectors.userEntry = '.user-entry';
        selectors.artStyle = '#ArtStyle';
        selectors.soundStyle = '#SoundStyle';
        selectors.rowConcept = '.row-concept';
        selectors.rowCharacter = '.row-character';
        selectors.rowLevel = '.row-level';
        selectors.rowSoundFx = '.row-soundfx';
        selectors.rowMusic = '.row-music';
        selectors.tableValue = '.table-value';
        selectors.btnCalculate = '#btnCalculate';

        selectors.conceptArtCount = '#ConceptArtCount';
        selectors.characterCount = '#CharacterCount2d';
        selectors.levelCount = '#LevelCount2d';
        selectors.soundFxCount = '#SoundEffectCount';
        selectors.musicCount = '#MusicTrackCount';

        selectors.txtConceptQuantity = '#txtConceptQuantity';
        selectors.txtCharacterQuantity = '#txtCharacterQuantity';
        selectors.txtLevelQuantity = '#txtLevelQuantity';
        selectors.txtSoundFxQuantity = '#txtSoundFxQuantity';
        selectors.txtMusicQuantity = '#txtMusicQuantity';

        selectors.txtConceptMinimum = '#txtConceptMinimum';
        selectors.txtCharacterMinimum = '#txtCharacterMinimum';
        selectors.txtLevelMinimum = '#txtLevelMinimum';
        selectors.txtSoundFxMinimum = '#txtSoundFxMinimum';
        selectors.txtMusicMinimum = '#txtMusicMinimum';

        selectors.txtConceptAverage = '#txtConceptAverage';
        selectors.txtCharacterAverage = '#txtCharacterAverage';
        selectors.txtLevelAverage = '#txtLevelAverage';
        selectors.txtSoundFxAverage = '#txtSoundFxAverage';
        selectors.txtMusicAverage = '#txtMusicAverage';

        selectors.txtConceptMaximum = '#txtConceptMaximum';
        selectors.txtCharacterMaximum = '#txtCharacterMaximum';
        selectors.txtLevelMaximum = '#txtLevelMaximum';
        selectors.txtSoundFxMaximum = '#txtSoundFxMaximum';
        selectors.txtMusicMaximum = '#txtMusicMaximum';

        selectors.txtTotalQuantity = '#txtTotalQuantity';
        selectors.txtTotalMinimum = '#txtTotalMinimum';
        selectors.txtTotalAverage = '#txtTotalAverage';
        selectors.txtTotalMaximum = '#txtTotalMaximum';
    }

    function cacheObjs() {
        objs.urls = $(selectors.urls);
        objs.container = $(selectors.container);
        objs.form = $(selectors.form);
        objs.artStyle = $(selectors.artStyle);
        objs.soundStyle = $(selectors.soundStyle);

        objs.conceptArtCount = $(selectors.conceptArtCount);
        objs.characterCount = $(selectors.characterCount);
        objs.levelCount = $(selectors.levelCount);
        objs.soundFxCount = $(selectors.soundFxCount);
        objs.musicCount = $(selectors.musicCount);

        objs.txtConceptQuantity = $(selectors.txtConceptQuantity);
        objs.txtCharacterQuantity = $(selectors.txtCharacterQuantity);
        objs.txtLevelQuantity = $(selectors.txtLevelQuantity);
        objs.txtSoundFxQuantity = $(selectors.txtSoundFxQuantity);
        objs.txtMusicQuantity = $(selectors.txtMusicQuantity);

        objs.txtConceptMinimum = $(selectors.txtConceptMinimum);
        objs.txtCharacterMinimum = $(selectors.txtCharacterMinimum);
        objs.txtLevelMinimum = $(selectors.txtLevelMinimum);
        objs.txtSoundFxMinimum = $(selectors.txtSoundFxMinimum);
        objs.txtMusicMinimum = $(selectors.txtMusicMinimum);

        objs.txtConceptAverage = $(selectors.txtConceptAverage);
        objs.txtCharacterAverage = $(selectors.txtCharacterAverage);
        objs.txtLevelAverage = $(selectors.txtLevelAverage);
        objs.txtSoundFxAverage = $(selectors.txtSoundFxAverage);
        objs.txtMusicAverage = $(selectors.txtMusicAverage);

        objs.txtConceptMaximum = $(selectors.txtConceptMaximum);
        objs.txtCharacterMaximum = $(selectors.txtCharacterMaximum);
        objs.txtLevelMaximum = $(selectors.txtLevelMaximum);
        objs.txtSoundFxMaximum = $(selectors.txtSoundFxMaximum);
        objs.txtMusicMaximum = $(selectors.txtMusicMaximum);

        objs.txtTotalQuantity = $(selectors.txtTotalQuantity);
        objs.txtTotalMinimum = $(selectors.txtTotalMinimum);
        objs.txtTotalAverage = $(selectors.txtTotalAverage);
        objs.txtTotalMaximum = $(selectors.txtTotalMaximum);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        MAINMODULE.Common.BindPopOvers();

        getRates(styleChange);

        setQuantities();

        initRangeSlider(selectors.container);
    }

    function bindAll() {
        bindStyleChange();
        bindBtnCalculate();
    }

    function bindStyleChange() {
        objs.container.on('change', selectors.artStyle, styleChange);

        objs.container.on('change', selectors.soundStyle, styleChange);
    }

    function styleChange() {
        selectedArtStyle = parseInt(objs.artStyle.val(), 10);
        selectedSoundStyle = parseInt(objs.soundStyle.val(), 10);

        calculate();
    }

    function bindBtnCalculate() {
        objs.container.on('click', selectors.btnCalculate, function () {
            calculate();
        });
    }

    function getRates(callback) {
        var url = objs.urls.data('urlGetRates');
        MAINMODULE.Ajax.Get(url, function (response) {
            if (response.success) {
                rates = response.value;

                if (callback) {
                    callback();
                }
            }
        });
    }

    function formatTableText(priceValue, timeValue, quantity) {
        var multiplier = priceValue * timeValue;
        return quantity === 0 || priceValue === 0 ? 0 : `(x${multiplier}) ${multiplier * quantity}`
    }

    function setRates() {

        var conceptArtRates = rates.visualRates.filter(x => x.gameElement === objs.conceptArtCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (conceptArtRates.length === 0) {
            $(selectors.rowConcept + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumConceptCost = conceptArtRates[0].price.minimum * conceptArtRates[0].time.minimum * conceptCount;
            averageConceptCost = conceptArtRates[0].price.average * conceptArtRates[0].time.average * conceptCount;
            maximumConceptCost = conceptArtRates[0].price.maximum * conceptArtRates[0].time.maximum * conceptCount;

            objs.txtConceptMinimum.text(formatTableText(conceptArtRates[0].price.minimum, conceptArtRates[0].time.minimum, conceptCount));
            objs.txtConceptAverage.text(formatTableText(conceptArtRates[0].price.average, conceptArtRates[0].time.average, conceptCount));
            objs.txtConceptMaximum.text(formatTableText(conceptArtRates[0].price.maximum, conceptArtRates[0].time.maximum, conceptCount));
        }

        var characterRates = rates.visualRates.filter(x => x.gameElement === objs.characterCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (characterRates.length === 0) {
            $(selectors.rowCharacter + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumCharacterCost = characterRates[0].price.minimum * characterRates[0].time.minimum * characterCount;
            averageCharacterCost = characterRates[0].price.average * characterRates[0].time.average * characterCount;
            maximumCharacterCost = characterRates[0].price.maximum * characterRates[0].time.maximum * characterCount;

            objs.txtCharacterMinimum.text(formatTableText(characterRates[0].price.minimum, characterRates[0].time.minimum, characterCount));
            objs.txtCharacterAverage.text(formatTableText(characterRates[0].price.average, characterRates[0].time.average, characterCount));
            objs.txtCharacterMaximum.text(formatTableText(characterRates[0].price.maximum, characterRates[0].time.maximum, characterCount));
        }

        var levelRates = rates.visualRates.filter(x => x.gameElement === objs.levelCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (levelRates.length === 0) {
            $(selectors.rowLevel + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumLevelCost = levelRates[0].price.minimum * levelRates[0].time.minimum * levelCount;
            averageLevelCost = levelRates[0].price.average * levelRates[0].time.average * levelCount;
            maximumLevelCost = levelRates[0].price.maximum * levelRates[0].time.maximum * levelCount;

            objs.txtLevelMinimum.text(formatTableText(levelRates[0].price.minimum, levelRates[0].time.minimum, levelCount));
            objs.txtLevelAverage.text(formatTableText(levelRates[0].price.average, levelRates[0].time.average, levelCount));
            objs.txtLevelMaximum.text(formatTableText(levelRates[0].price.maximum, levelRates[0].time.maximum, levelCount));
        }

        var soundFxRates = rates.audioRates.filter(x => x.gameElement === objs.soundFxCount.data('gameelement') && x.soundStyle === selectedSoundStyle);

        if (soundFxRates.length === 0) {
            $(selectors.rowSoundFx + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumSoundFxCost = soundFxRates[0].price.minimum * soundFxRates[0].time.minimum * soundFxCount;
            averageSoundFxCost = soundFxRates[0].price.average * soundFxRates[0].time.average * soundFxCount;
            maximumSoundFxCost = soundFxRates[0].price.maximum * soundFxRates[0].time.maximum * soundFxCount;

            objs.txtSoundFxMinimum.text(formatTableText(soundFxRates[0].price.minimum, soundFxRates[0].time.minimum, soundFxCount));
            objs.txtSoundFxAverage.text(formatTableText(soundFxRates[0].price.average, soundFxRates[0].time.average, soundFxCount));
            objs.txtSoundFxMaximum.text(formatTableText(soundFxRates[0].price.maximum, soundFxRates[0].time.maximum, soundFxCount));
        }

        var musicRates = rates.audioRates.filter(x => x.gameElement === objs.musicCount.data('gameelement') && x.soundStyle === selectedSoundStyle);

        if (musicRates.length === 0) {
            $(selectors.rowMusic + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumMusicCost = musicRates[0].price.minimum * musicRates[0].time.minimum * musicCount;
            averageMusicCost = musicRates[0].price.average * musicRates[0].time.average * musicCount;
            maximumMusicCost = musicRates[0].price.maximum * musicRates[0].time.maximum * musicCount;

            objs.txtMusicMinimum.text(formatTableText(musicRates[0].price.minimum, musicRates[0].time.minimum, musicCount));
            objs.txtMusicAverage.text(formatTableText(musicRates[0].price.average, musicRates[0].time.average, musicCount));
            objs.txtMusicMaximum.text(formatTableText(musicRates[0].price.maximum, musicRates[0].time.maximum, musicCount));
        }

        var finalElemensCount = conceptCount + characterCount + levelCount + soundFxCount + musicCount;

        var finalMinimumCost = minimumConceptCost + minimumCharacterCost + minimumLevelCost + minimumSoundFxCost + minimumMusicCost;
        var finalAverageCost = averageConceptCost + averageCharacterCost + averageLevelCost + averageSoundFxCost + averageMusicCost;
        var finalMaximumCost = maximumConceptCost + maximumCharacterCost + maximumLevelCost + maximumSoundFxCost + maximumMusicCost;

        objs.txtTotalQuantity.text(finalElemensCount);
        objs.txtTotalMinimum.text(finalMinimumCost);
        objs.txtTotalAverage.text(finalAverageCost);
        objs.txtTotalMaximum.text(finalMaximumCost);
    }

    function setQuantities() {
        conceptCount = parseInt(objs.conceptArtCount.val() || 0, 10);
        characterCount = parseInt(objs.characterCount.val() || 0, 10);
        levelCount = parseInt(objs.levelCount.val() || 0, 10);
        soundFxCount = parseInt(objs.soundFxCount.val() || 0, 10);
        musicCount = parseInt(objs.musicCount.val() || 0, 10);

        objs.txtConceptQuantity.text(conceptCount);
        objs.txtCharacterQuantity.text(characterCount);
        objs.txtLevelQuantity.text(levelCount);
        objs.txtSoundFxQuantity.text(soundFxCount);
        objs.txtMusicQuantity.text(musicCount);
    }

    function calculate() {
        $(selectors.tableValue).fadeOut('fast', function () {
            setQuantities();
            setRates();
            $(selectors.tableValue).fadeIn();
        });
        
    }



    const initRangeSlider = (context) => {
        $('input[type="range"]', context).each(function () {
            var slider = $(this);
            var handle;

            slider.rangeslider({
                polyfill: false,
                onInit: function (position, value) {
                    handle = setSliderHandler(handle, this.$range, this.value);
                },
                onSlide: function (position, value) {
                    handle.html(this.value);
                }
            });
        });
    };


    function setSliderHandler(handle, range, value) {
        if (handle === undefined) {
            handle = $('.rangeslider__handle', range);
        }

        handle.html(value);

        return handle;
    }

    return {
        Init: init
    };
}());

$(function () {
    COSTCALCULATOR.Init();
});