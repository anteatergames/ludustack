var RATECALCULATOR = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    function setSelectors() {
        selectors.container = '#featurecontainer';
        selectors.form = '#frmRateCalculator';
        selectors.userEntry = '.user-entry';
        selectors.yearWorkDays = '#YearWorkDays';
        selectors.monthlyExpenses = '#MontlyExpenses';
        selectors.annualCost = '#AnnualCost';
        selectors.savingsPercentage = '#SavingsPercentage';
        selectors.savingsValueMonthly = '#SavingsValueMonthly';
        selectors.savingsValueYearly = '#SavingsValueYearly';
        selectors.yourExtraPercentage = '#YourExtraPercentage';
        selectors.yourExtraValueMonthly = '#YourExtraValueMontly';
        selectors.yourExtraValueYearly = '#YourExtraValueYearly';
        selectors.taxesPercentage = '#TaxesPercentage';
        selectors.taxesValueMonthly = '#TaxesValueMontly';
        selectors.taxesValueYearly = '#TaxesValueYearly';
        selectors.revisionFactor = '#RevisionFactor';
        selectors.rushFactor = '#RushFactor';
        selectors.assholeFactor = '#AssholeFactor';
        selectors.txtPerDay = '#txtPerDay';
        selectors.txtPerHour = '#txtPerHour';
        selectors.txtPerRevisionDay = '#txtPerRevisionDay';
        selectors.txtPerRevisionHour = '#txtPerRevisionHour';
        selectors.txtPerRushJobDay = '#txtPerRushJobDay';
        selectors.txtPerRushJobHour = '#txtPerRushJobHour';
        selectors.txtAssholeRateDay = '#txtAssholeRateDay';
        selectors.txtAssholeRateHour = '#txtAssholeRateHour';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.form = $(selectors.form);
        objs.yearWorkDays = $(selectors.yearWorkDays);
        objs.monthlyExpenses = $(selectors.monthlyExpenses);
        objs.annualCost = $(selectors.annualCost);
        objs.savingsPercentage = $(selectors.savingsPercentage);
        objs.savingsValueMonthly = $(selectors.savingsValueMonthly);
        objs.savingsValueYearly = $(selectors.savingsValueYearly);
        objs.yourExtraPercentage = $(selectors.yourExtraPercentage);
        objs.yourExtraValueMonthly = $(selectors.yourExtraValueMonthly);
        objs.yourExtraValueYearly = $(selectors.yourExtraValueYearly);
        objs.taxesPercentage = $(selectors.taxesPercentage);
        objs.taxesValueMonthly = $(selectors.taxesValueMonthly);
        objs.taxesValueYearly = $(selectors.taxesValueYearly);
        objs.revisionFactor = $(selectors.revisionFactor);
        objs.rushFactor = $(selectors.rushFactor);
        objs.assholeFactor = $(selectors.assholeFactor);
        objs.txtPerDay = $(selectors.txtPerDay);
        objs.txtPerHour = $(selectors.txtPerHour);
        objs.txtPerRevisionDay = $(selectors.txtPerRevisionDay);
        objs.txtPerRevisionHour = $(selectors.txtPerRevisionHour);
        objs.txtPerRushJobDay = $(selectors.txtPerRushJobDay);
        objs.txtPerRushJobHour = $(selectors.txtPerRushJobHour);
        objs.txtAssholeRateDay = $(selectors.txtAssholeRateDay);
        objs.txtAssholeRateHour = $(selectors.txtAssholeRateHour);
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        MAINMODULE.Common.BindPopOvers();

        objs.monthlyExpenses.focus().select();
    }

    function bindAll() {
        bindMontlyChange();
        bindAnnualChange();
        bindSavingsChange();
        bindYourExtraChange();
        bindTaxesChange();

        bindInputsChange();
    }

    function bindInputsChange() {
        objs.container.on('change', selectors.userEntry, function () {
            calculate();
        });
    }

    function bindMontlyChange() {
        objs.container.on('change', selectors.monthlyExpenses, function () {
            var val = $(this).val();

            var annual = val * 12;

            objs.annualCost.val(annual.toFixed(2));
        });
    }

    function bindAnnualChange() {
        objs.container.on('change', selectors.annualCost, function () {
            var val = $(this).val();

            var montly = val / 12;

            objs.monthlyExpenses.val(montly.toFixed(2));
        });
    }

    function bindSavingsChange() {
        objs.container.on('change', selectors.savingsPercentage, function () {
            calculate();
        });
    }

    function bindYourExtraChange() {
        objs.container.on('change', selectors.yourExtraPercentage, function () {
            calculate();
        });
    }

    function bindTaxesChange() {
        objs.container.on('change', selectors.taxesPercentage, function () {
            calculate();
        });
    }

    function calculate() {
        var savingsValueMonthly = objs.monthlyExpenses.val() * objs.savingsPercentage.val() / 100;

        objs.savingsValueMonthly.val(savingsValueMonthly.toFixed(2))
        objs.savingsValueYearly.val((savingsValueMonthly * 12).toFixed(2));

        var yourExtraValueMonthly = objs.monthlyExpenses.val() * objs.yourExtraPercentage.val() / 100;

        objs.yourExtraValueMonthly.val(yourExtraValueMonthly.toFixed(2))
        objs.yourExtraValueYearly.val((yourExtraValueMonthly * 12).toFixed(2));

        var taxesValueMonthly = objs.monthlyExpenses.val() * objs.taxesPercentage.val() / 100;

        objs.taxesValueMonthly.val(taxesValueMonthly.toFixed(2))
        objs.taxesValueYearly.val((taxesValueMonthly * 12).toFixed(2));

        if (objs.yearWorkDays.val().length === 0) {
            objs.yearWorkDays.val('231');
        }

        var dayRate = objs.annualCost.val() / objs.yearWorkDays.val();
        var hourRate = dayRate / 8;

        var revisionFactor = (objs.revisionFactor.val() / 100) + 1;
        var revisionDayRate = dayRate * revisionFactor;
        var revisionHourRate = revisionDayRate / 8;

        var rushFactor = (objs.rushFactor.val() / 100) + 1;
        var rushFactorDayRate = dayRate * rushFactor;
        var rushHourRate = rushFactorDayRate / 8;

        var assHoleFactor = (objs.assholeFactor.val() / 100) + 1;
        var assHoleFactorDayRate = dayRate * assHoleFactor;
        var assHoleHourRate = assHoleFactorDayRate / 8;

        objs.txtPerDay.html(dayRate.toFixed(2));
        objs.txtPerHour.html(hourRate.toFixed(2));
        objs.txtPerRevisionDay.html(revisionDayRate.toFixed(2));
        objs.txtPerRevisionHour.html(revisionHourRate.toFixed(2));
        objs.txtPerRushJobDay.html(rushFactorDayRate.toFixed(2));
        objs.txtPerRushJobHour.html(rushHourRate.toFixed(2));
        objs.txtAssholeRateDay.html(assHoleFactorDayRate.toFixed(2));
        objs.txtAssholeRateHour.html(assHoleHourRate.toFixed(2));
    }

    return {
        Init: init
    };
}());

$(function () {
    RATECALCULATOR.Init();
});