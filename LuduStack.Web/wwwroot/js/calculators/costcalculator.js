var COSTCALCULATOR = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var rates;
    var selectedArtStyle;
    var selectedSoundStyle;

    var conceptCount;
    var character2dCount;
    var asset2dCount;
    var level2dCount;
    var character3dCount;
    var asset3dCount;
    var level3dCount;
    var soundFxCount;
    var musicCount;

    var minimumConceptCost = 0;
    var minimumCharacter2dCost = 0;
    var minimumAsset2dCost = 0;
    var minimumLevel2dCost = 0;
    var minimumCharacter3dCost = 0;
    var minimumAsset3dCost = 0;
    var minimumLevel3dCost = 0;
    var minimumSoundFxCost = 0;
    var minimumMusicCost = 0;

    var averageConceptCost = 0;
    var averageCharacter2dCost = 0;
    var averageAsset2dCost = 0;
    var averageLevel2dCost = 0;
    var averageCharacter3dCost = 0;
    var averageAsset3dCost = 0;
    var averageLevel3dCost = 0;
    var averageSoundFxCost = 0;
    var averageMusicCost = 0;

    var maximumConceptCost = 0;
    var maximumCharacter2dCost = 0;
    var maximumAsset2dCost = 0;
    var maximumLevel2dCost = 0;
    var maximumCharacter3dCost = 0;
    var maximumAsset3dCost = 0;
    var maximumLevel3dCost = 0;
    var maximumSoundFxCost = 0;
    var maximumMusicCost = 0;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmRateCalculator';
        selectors.userEntry = '.user-entry';
        selectors.visualDimensions = '#VisualDimensions';
        selectors.artStyle = '#ArtStyle';
        selectors.soundStyle = '#SoundStyle';
        selectors.rowConcept = '.row-concept';
        selectors.ro2dwCharacter = '.row-character';
        selectors.rowLevel2d = '.row-level';
        selectors.rowSoundFx = '.row-soundfx';
        selectors.rowMusic = '.row-music';
        selectors.tableValue = '.table-value';
        selectors.btnCalculate = '#btnCalculate';
        selectors.artPerDimension = '.art-per-dimension';

        selectors.conceptArtCount = '#ConceptArtCount';
        selectors.character2dCount = '#CharacterCount2d';
        selectors.asset2dCount = '#AssetCount2d';
        selectors.level2dCount = '#LevelCount2d';
        selectors.character3dCount = '#CharacterCount3d';
        selectors.asset3dCount = '#AssetCount3d';
        selectors.level3dCount = '#LevelCount3d';
        selectors.soundFxCount = '#SoundEffectCount';
        selectors.musicCount = '#MusicTrackCount';

        selectors.txtConceptQuantity = '#txtConceptQuantity';
        selectors.txtCharacter2dQuantity = '#txtCharacter2dQuantity';
        selectors.txtAsset2dQuantity = '#txtAsset2dQuantity';
        selectors.txtLevel2dQuantity = '#txtLevel2dQuantity';
        selectors.txtCharacter3dQuantity = '#txtCharacter3dQuantity';
        selectors.txtAsset3dQuantity = '#txtAsset3dQuantity';
        selectors.txtLevel3dQuantity = '#txtLevel3dQuantity';
        selectors.txtSoundFxQuantity = '#txtSoundFxQuantity';
        selectors.txtMusicQuantity = '#txtMusicQuantity';

        selectors.txtConceptMinimum = '#txtConceptMinimum';
        selectors.txtCharacter2dMinimum = '#txtCharacter2dMinimum';
        selectors.txtAsset2dMinimum = '#txtAsset2dMinimum';
        selectors.txtLevel2dMinimum = '#txtLevel2dMinimum';
        selectors.txtCharacter3dMinimum = '#txtCharacter3dMinimum';
        selectors.txtAsset3dMinimum = '#txtAsset3dMinimum';
        selectors.txtLevel3dMinimum = '#txtLevel3dMinimum';
        selectors.txtSoundFxMinimum = '#txtSoundFxMinimum';
        selectors.txtMusicMinimum = '#txtMusicMinimum';

        selectors.txtConceptAverage = '#txtConceptAverage';
        selectors.txtCharacter2dAverage = '#txtCharacter2dAverage';
        selectors.txtAsset2dAverage = '#txtAsset2dAverage';
        selectors.txtLevel2dAverage = '#txtLevel2dAverage';
        selectors.txtCharacter3dAverage = '#txtCharacter3dAverage';
        selectors.txtAsset3dAverage = '#txtAsset3dAverage';
        selectors.txtLevel3dAverage = '#txtLevel3dAverage';
        selectors.txtSoundFxAverage = '#txtSoundFxAverage';
        selectors.txtMusicAverage = '#txtMusicAverage';

        selectors.txtConceptMaximum = '#txtConceptMaximum';
        selectors.txtCharacter2dMaximum = '#txtCharacter2dMaximum';
        selectors.txtAsset2dMaximum = '#txtAsset2dMaximum';
        selectors.txtLevel2dMaximum = '#txtLevel2dMaximum';
        selectors.txtCharacter3dMaximum = '#txtCharacter3dMaximum';
        selectors.txtAsset3dMaximum = '#txtAsset3dMaximum';
        selectors.txtLevel3dMaximum = '#txtLevel3dMaximum';
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
        objs.visualDimensions = $(selectors.visualDimensions);
        objs.artStyle = $(selectors.artStyle);
        objs.soundStyle = $(selectors.soundStyle);
        objs.artPerDimension = $(selectors.artPerDimension);

        objs.conceptArtCount = $(selectors.conceptArtCount);
        objs.character2dCount = $(selectors.character2dCount);
        objs.asset2dCount = $(selectors.asset2dCount);
        objs.level2dCount = $(selectors.level2dCount);
        objs.character3dCount = $(selectors.character3dCount);
        objs.asset3dCount = $(selectors.asset3dCount);
        objs.level3dCount = $(selectors.level3dCount);
        objs.soundFxCount = $(selectors.soundFxCount);
        objs.musicCount = $(selectors.musicCount);

        objs.txtConceptQuantity = $(selectors.txtConceptQuantity);
        objs.txtCharacter2dQuantity = $(selectors.txtCharacter2dQuantity);
        objs.txtAsset2dQuantity = $(selectors.txtAsset2dQuantity);
        objs.txtLevel2dQuantity = $(selectors.txtLevel2dQuantity);
        objs.txtCharacter3dQuantity = $(selectors.txtCharacter3dQuantity);
        objs.txtAsset3dQuantity = $(selectors.txtAsset3dQuantity);
        objs.txtLevel3dQuantity = $(selectors.txtLevel3dQuantity);
        objs.txtSoundFxQuantity = $(selectors.txtSoundFxQuantity);
        objs.txtMusicQuantity = $(selectors.txtMusicQuantity);

        objs.txtConceptMinimum = $(selectors.txtConceptMinimum);
        objs.txtCharacter2dMinimum = $(selectors.txtCharacter2dMinimum);
        objs.txtAsset2dMinimum = $(selectors.txtAsset2dMinimum);
        objs.txtLevel2dMinimum = $(selectors.txtLevel2dMinimum);
        objs.txtCharacter3dMinimum = $(selectors.txtCharacter3dMinimum);
        objs.txtAsset3dMinimum = $(selectors.txtAsset3dMinimum);
        objs.txtLevel3dMinimum = $(selectors.txtLevel3dMinimum);
        objs.txtSoundFxMinimum = $(selectors.txtSoundFxMinimum);
        objs.txtMusicMinimum = $(selectors.txtMusicMinimum);

        objs.txtConceptAverage = $(selectors.txtConceptAverage);
        objs.txtCharacter2dAverage = $(selectors.txtCharacter2dAverage);
        objs.txtAsset2dAverage = $(selectors.txtAsset2dAverage);
        objs.txtLevel2dAverage = $(selectors.txtLevel2dAverage);
        objs.txtCharacter3dAverage = $(selectors.txtCharacter3dAverage);
        objs.txtAsset3dAverage = $(selectors.txtAsset3dAverage);
        objs.txtLevel3dAverage = $(selectors.txtLevel3dAverage);
        objs.txtSoundFxAverage = $(selectors.txtSoundFxAverage);
        objs.txtMusicAverage = $(selectors.txtMusicAverage);

        objs.txtConceptMaximum = $(selectors.txtConceptMaximum);
        objs.txtCharacter2dMaximum = $(selectors.txtCharacter2dMaximum);
        objs.txtAsset2dMaximum = $(selectors.txtAsset2dMaximum);
        objs.txtLevel2dMaximum = $(selectors.txtLevel2dMaximum);
        objs.txtCharacter3dMaximum = $(selectors.txtCharacter3dMaximum);
        objs.txtAsset3dMaximum = $(selectors.txtAsset3dMaximum);
        objs.txtLevel3dMaximum = $(selectors.txtLevel3dMaximum);
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

        dimensionChange(objs.visualDimensions.val());

        bindAll();

        MAINMODULE.Common.BindPopOvers();

        getRates(styleChange);

        initRangeSlider(selectors.container);
    }

    function bindAll() {
        bindDimensionChange();
        bindStyleChange();
        bindBtnCalculate();
    }

    function bindDimensionChange() {
        objs.container.on('change', selectors.visualDimensions, function (e) {
            dimensionChange(e.target.value);
        });
    }

    function bindStyleChange() {
        objs.container.on('change', selectors.artStyle, styleChange);

        objs.container.on('change', selectors.soundStyle, styleChange);
    }

    function dimensionChange(value) {
        var theOthers = $(selectors.artPerDimension + '[data-subtype!="' + value + '"]' + ' input');

        $(selectors.artPerDimension + '[data-subtype!="' + value + '"]').css('visibility', 'hidden').hide();
        $(selectors.artPerDimension + '[data-subtype="' + value + '"]').css('visibility', 'visible').fadeIn('slow');

        theOthers.val(0).change();
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
        return quantity === 0 || priceValue === 0 ? '0' : `(x${multiplier}) ${multiplier * quantity}`;
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

        var character2dRates = rates.visualRates.filter(x => x.gameElement === objs.character2dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (character2dRates.length === 0) {
            $(selectors.rowCharacter2d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumCharacter2dCost = character2dRates[0].price.minimum * character2dRates[0].time.minimum * character2dCount;
            averageCharacter2dCost = character2dRates[0].price.average * character2dRates[0].time.average * character2dCount;
            maximumCharacter2dCost = character2dRates[0].price.maximum * character2dRates[0].time.maximum * character2dCount;

            objs.txtCharacter2dMinimum.text(formatTableText(character2dRates[0].price.minimum, character2dRates[0].time.minimum, character2dCount));
            objs.txtCharacter2dAverage.text(formatTableText(character2dRates[0].price.average, character2dRates[0].time.average, character2dCount));
            objs.txtCharacter2dMaximum.text(formatTableText(character2dRates[0].price.maximum, character2dRates[0].time.maximum, character2dCount));
        }

        var asset2dRates = rates.visualRates.filter(x => x.gameElement === objs.asset2dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (asset2dRates.length === 0) {
            $(selectors.rowAsset2d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumAsset2dCost = asset2dRates[0].price.minimum * asset2dRates[0].time.minimum * asset2dCount;
            averageAsset2dCost = asset2dRates[0].price.average * asset2dRates[0].time.average * asset2dCount;
            maximumAsset2dCost = asset2dRates[0].price.maximum * asset2dRates[0].time.maximum * asset2dCount;

            objs.txtAsset2dMinimum.text(formatTableText(asset2dRates[0].price.minimum, asset2dRates[0].time.minimum, asset2dCount));
            objs.txtAsset2dAverage.text(formatTableText(asset2dRates[0].price.average, asset2dRates[0].time.average, asset2dCount));
            objs.txtAsset2dMaximum.text(formatTableText(asset2dRates[0].price.maximum, asset2dRates[0].time.maximum, asset2dCount));
        }

        var level2dRates = rates.visualRates.filter(x => x.gameElement === objs.level2dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (level2dRates.length === 0) {
            $(selectors.rowLevel2d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumLevel2dCost = level2dRates[0].price.minimum * level2dRates[0].time.minimum * level2dCount;
            averageLevel2dCost = level2dRates[0].price.average * level2dRates[0].time.average * level2dCount;
            maximumLevel2dCost = level2dRates[0].price.maximum * level2dRates[0].time.maximum * level2dCount;

            objs.txtLevel2dMinimum.text(formatTableText(level2dRates[0].price.minimum, level2dRates[0].time.minimum, level2dCount));
            objs.txtLevel2dAverage.text(formatTableText(level2dRates[0].price.average, level2dRates[0].time.average, level2dCount));
            objs.txtLevel2dMaximum.text(formatTableText(level2dRates[0].price.maximum, level2dRates[0].time.maximum, level2dCount));
        }

        var character3dRates = rates.visualRates.filter(x => x.gameElement === objs.character3dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (character3dRates.length === 0) {
            $(selectors.rowCharacter3d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumCharacter3dCost = character3dRates[0].price.minimum * character3dRates[0].time.minimum * character3dCount;
            averageCharacter3dCost = character3dRates[0].price.average * character3dRates[0].time.average * character3dCount;
            maximumCharacter3dCost = character3dRates[0].price.maximum * character3dRates[0].time.maximum * character3dCount;

            objs.txtCharacter3dMinimum.text(formatTableText(character3dRates[0].price.minimum, character3dRates[0].time.minimum, character3dCount));
            objs.txtCharacter3dAverage.text(formatTableText(character3dRates[0].price.average, character3dRates[0].time.average, character3dCount));
            objs.txtCharacter3dMaximum.text(formatTableText(character3dRates[0].price.maximum, character3dRates[0].time.maximum, character3dCount));
        }

        var asset3dRates = rates.visualRates.filter(x => x.gameElement === objs.asset3dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (asset3dRates.length === 0) {
            $(selectors.rowAsset3d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumAsset3dCost = asset3dRates[0].price.minimum * asset3dRates[0].time.minimum * asset3dCount;
            averageAsset3dCost = asset3dRates[0].price.average * asset3dRates[0].time.average * asset3dCount;
            maximumAsset3dCost = asset3dRates[0].price.maximum * asset3dRates[0].time.maximum * asset3dCount;

            objs.txtAsset3dMinimum.text(formatTableText(asset3dRates[0].price.minimum, asset3dRates[0].time.minimum, asset3dCount));
            objs.txtAsset3dAverage.text(formatTableText(asset3dRates[0].price.average, asset3dRates[0].time.average, asset3dCount));
            objs.txtAsset3dMaximum.text(formatTableText(asset3dRates[0].price.maximum, asset3dRates[0].time.maximum, asset3dCount));
        }

        var level3dRates = rates.visualRates.filter(x => x.gameElement === objs.level3dCount.data('gameelement') && x.artStyle === selectedArtStyle);

        if (level3dRates.length === 0) {
            $(selectors.rowLevel3d + ' ' + selectors.tableValue).text('-');
        }
        else {
            minimumLevel3dCost = level3dRates[0].price.minimum * level3dRates[0].time.minimum * level3dCount;
            averageLevel3dCost = level3dRates[0].price.average * level3dRates[0].time.average * level3dCount;
            maximumLevel3dCost = level3dRates[0].price.maximum * level3dRates[0].time.maximum * level3dCount;

            objs.txtLevel3dMinimum.text(formatTableText(level3dRates[0].price.minimum, level3dRates[0].time.minimum, level3dCount));
            objs.txtLevel3dAverage.text(formatTableText(level3dRates[0].price.average, level3dRates[0].time.average, level3dCount));
            objs.txtLevel3dMaximum.text(formatTableText(level3dRates[0].price.maximum, level3dRates[0].time.maximum, level3dCount));
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

        var finalElemensCount = conceptCount + character2dCount + asset2dCount + level2dCount + soundFxCount + musicCount;

        var finalMinimumCost = minimumConceptCost + minimumCharacter2dCost + minimumAsset2dCost + minimumLevel2dCost + minimumCharacter3dCost + minimumAsset3dCost + minimumLevel3dCost + minimumSoundFxCost + minimumMusicCost;
        var finalAverageCost = averageConceptCost + averageCharacter2dCost + averageAsset2dCost + averageLevel2dCost + averageCharacter3dCost + averageAsset3dCost + averageLevel3dCost + averageSoundFxCost + averageMusicCost;
        var finalMaximumCost = maximumConceptCost + maximumCharacter2dCost + maximumAsset2dCost + maximumLevel2dCost + maximumCharacter3dCost + maximumAsset3dCost + maximumLevel3dCost + maximumSoundFxCost + maximumMusicCost;

        objs.txtTotalQuantity.text(finalElemensCount);
        objs.txtTotalMinimum.text(finalMinimumCost);
        objs.txtTotalAverage.text(finalAverageCost);
        objs.txtTotalMaximum.text(finalMaximumCost);
    }

    function setQuantities() {
        conceptCount = parseInt(objs.conceptArtCount.val() || 0, 10);
        character2dCount = parseInt(objs.character2dCount.val() || 0, 10);
        asset2dCount = parseInt(objs.asset2dCount.val() || 0, 10);
        level2dCount = parseInt(objs.level2dCount.val() || 0, 10);
        character3dCount = parseInt(objs.character3dCount.val() || 0, 10);
        asset3dCount = parseInt(objs.asset3dCount.val() || 0, 10);
        level3dCount = parseInt(objs.level3dCount.val() || 0, 10);
        soundFxCount = parseInt(objs.soundFxCount.val() || 0, 10);
        musicCount = parseInt(objs.musicCount.val() || 0, 10);

        objs.txtConceptQuantity.text(conceptCount);
        objs.txtCharacter2dQuantity.text(character2dCount);
        objs.txtAsset2dQuantity.text(asset2dCount);
        objs.txtLevel2dQuantity.text(level2dCount);
        objs.txtCharacter3dQuantity.text(character3dCount);
        objs.txtAsset3dQuantity.text(asset3dCount);
        objs.txtLevel3dQuantity.text(level3dCount);
        objs.txtSoundFxQuantity.text(soundFxCount);
        objs.txtMusicQuantity.text(musicCount);
    }

    function calculate() {
        $.when($(selectors.tableValue).fadeOut('fast')).then(function () {
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