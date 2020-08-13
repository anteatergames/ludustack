var GAMEIDEA = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var clickSound;
    var slotsSound;

    var interval = 120;
    var stopInterval = 1200;

    var genreReady = false,
        actionReady = false,
        thingsReady = false,
        goalsReady = false,
        rulesReady = false,
        firstGenre = 0,
        firstAction = 0,
        firstThing = 0,
        firstGoal = 0,
        firstRule = 0,
        pickingGenreInterval = 0,
        stopGenreInterval = 0,
        pickingActionInterval = 0,
        stopActionInterval = 0,
        pickingThingsInterval = 0,
        stopThingsInterval = 0,
        pickingGoalsInterval = 0,
        stopGoalsInterval = 0,
        pickingRulesInterval = 0,
        stopRulesInterval = 0;

    var genre = ['battle royale', 'dungeon crawler', 'racing', 'action', 'arcade', 'educational', 'top-down', 'adventure', 'strategy', 'rts', 'turn-based strategy', 'role-playing', 'platformer', 'puzzle', 'visual novel', 'social media', 'mobile', 'browser', 'fighting', 'artsy', 'open world', 'tycoon', 'one touch'];

    var action = ['climb', 'break dance', 'skate', 'craft', 'lick', 'shoot at', 'play notes', 'grow', 'bounce', 'escape', 'rescue', 'go to war with', 'wage war on', 'unite', 'lead', 'build', 'destroy', 'conquer', 'invade', 'colonize', 'discover', 'explore', 'trade with', 'lead the rebels in', 'make peace with', 'investigate', 'rename', 'collect gold from', 'collect crystals from', 'mine ore from', 'align', 'click on', 'match', 'throw', 'toss', 'fire pellets at', 'control', 'touch', 'stack', 'guess', 'memorize', 'rotate', 'swap', 'slide', 'avoid', 'drag and drop', 'tickle', 'race', 'challenge', 'collect', 'draw', 'unlock', 'cook', 'break', 'solve puzzles involving', 'collect', 'juggle'];

    var things = ['rings', 'lasers', 'rusty pipes', 'life', 'dead people', 'tanks', 'punky androids', 'bikes', 'origami', 'walls', 'website', 'farm', 'ancient history', 'turtles', 'music notes', 'jigsaw pieces', 'robots', 'water', 'politician', 'colors', 'yo momma', 'countries', 'nations', 'dragons', 'castles', 'cities', 'strongholds', 'towers', 'dungeons', 'citadels', 'kingdoms', 'bombs', 'unknown worlds', 'other worlds', 'parallel worlds', 'other dimensions', 'alien worlds', 'heaven', 'hell', 'mythological places', 'historical places', 'islands', 'sanctuaries', 'temples', 'ruins', 'factories', 'caves', 'gems', 'diamonds', 'gold nuggets', 'bricks', 'bubbles', 'squares', 'triangles', 'treasure', 'blobs', 'kitchen appliances', 'nondescript fruits', 'animals', 'birds', 'baby animals', 'farm animals', 'exotic fruits', 'sentient plants', 'your friends', 'shapes', 'jewels', 'letters', 'words', 'numbers', 'tokens', 'coins', 'eggs', 'hats', 'candy', 'chocolate', 'shoes', 'clothing items', 'princesses', 'blocks', 'cubes', 'asteroids', 'stars', 'balls', 'spheres', 'magnets', 'riddles'];

    var goals = ['to survive', 'to win', 'for glory', 'in the name of love', 'to live forever', 'to rule the world', 'to form an empire', 'to win points', 'to reach the highscore', 'to unlock bonus items', 'to earn tokens', 'to unlock the next level', 'to become president', 'to earn discounts', 'to make your name', 'to pass exam', 'to never stop', 'to uncage a friend', 'to find the banana', 'to cook a meal'];

    var rules = ['avoid enemies', 'limited inventory', 'can\'t use twice', 'cannot avoid', 'must fly', 'one life only', 'must not be seen', 'can\'t touch the floor', 'limited time', 'must wait', 'can\'t breath', 'object is radioactive', 'the end is near', 'no gravity', 'naked', 'limited ammo', 'must dig', 'can\'t lose', 'can\'t regenerate', 'health is depleating', 'can\'t jump', 'silently'];


    function setSelectors() {
        selectors.gameIdeaStandalone = '#gameIdeaStandalone';
        selectors.btnGenerateGameIdea = '#btnGenerateGameIdea';
        selectors.genre = '.game-idea-genre';
        selectors.action = '.game-idea-action';
        selectors.things = '.game-idea-things';
        selectors.goals = '.game-idea-goals';
        selectors.rules = '.game-idea-rules';
    }

    function cacheObjs() {
        objs.gameIdeaStandalone = document.querySelector(selectors.gameIdeaStandalone);
        objs.btnGenerateGameIdea = document.querySelector(selectors.btnGenerateGameIdea);
        objs.genre = document.querySelector(selectors.genre);
        objs.action = document.querySelector(selectors.action);
        objs.things = document.querySelector(selectors.things);
        objs.goals = document.querySelector(selectors.goals);
        objs.rules = document.querySelector(selectors.rules);
    }

    function randomGenre() {
        objs.genre.classList.add("picking");
        pickingGenreInterval = setInterval(changeGenre, interval);

        stopGenreInterval = setInterval(stopPigkingGenre, stopInterval);
    }

    function changeGenre() {
        objs.genre.textContent = cleanElement(genre[firstGenre]);
        firstGenre = (firstGenre + 1) % genre.length;

        setVerticalPosition(objs.genre);
    }

    function stopPigkingGenre() {

        objs.genre.classList.remove("picking");
        clearInterval(pickingGenreInterval);
        clearInterval(stopGenreInterval);

        genreReady = true;
    }

    function randomAction() {
        objs.action.classList.add("picking");
        pickingActionInterval = setInterval(changeAction, interval);

        stopActionInterval = setInterval(stopPigkingAction, stopInterval);
    }

    function changeAction() {
        objs.action.textContent = cleanElement(action[firstAction]);
        firstAction = (firstAction + 1) % action.length;

        setVerticalPosition(objs.action);
    }

    function stopPigkingAction() {

        objs.action.classList.remove("picking");
        clearInterval(pickingActionInterval);
        clearInterval(stopActionInterval);

        actionReady = true;
    }

    function randomThings() {
        objs.things.classList.add("picking");
        pickingThingsInterval = setInterval(changeThings, interval);

        stopThingsInterval = setInterval(stopPigkingThings, stopInterval);
    }

    function changeThings() {
        objs.things.textContent = cleanElement(things[firstThing]);
        firstThing = (firstThing + 1) % things.length;

        setVerticalPosition(objs.things);
    }

    function stopPigkingThings() {

        objs.things.classList.remove("picking");
        clearInterval(pickingThingsInterval);
        clearInterval(stopThingsInterval);

        thingsReady = true;
    }

    function randomGoals() {
        objs.goals.classList.add("picking");
        pickingGoalsInterval = setInterval(changeGoals, interval);

        stopGoalsInterval = setInterval(stopPigkingGoals, stopInterval);
    }

    function changeGoals() {
        objs.goals.textContent = cleanElement(goals[firstGoal]);
        firstGoal = (firstGoal + 1) % goals.length;

        setVerticalPosition(objs.goals);
    }

    function stopPigkingGoals() {

        objs.goals.classList.remove("picking");
        clearInterval(pickingGoalsInterval);
        clearInterval(stopGoalsInterval);

        goalsReady = true;
    }

    function randomRules() {
        objs.rules.classList.add("picking");
        pickingRulesInterval = setInterval(changeRules, interval);

        stopRulesInterval = setInterval(stopPigkingRules, stopInterval);
    }

    function changeRules() {
        objs.rules.textContent = cleanElement(rules[firstRule]);
        firstRule = (firstRule + 1) % rules.length;

        setVerticalPosition(objs.rules);
    }

    function stopPigkingRules() {

        objs.rules.classList.remove("picking");
        clearInterval(pickingRulesInterval);
        clearInterval(stopRulesInterval);

        rulesReady = true;
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        if (objs.gameIdeaStandalone) {
            clickSound = new Audio("/sounds/click.mp3");
            slotsSound = new Audio("/sounds/gameideagenerator.mp3");
        }

        playSlotsSound();

        pick();
    }

    function bindAll() {
        bindGenerateClick();
    }

    function bindGenerateClick() {
        objs.btnGenerateGameIdea.addEventListener('click', function (e) {
            e.preventDefault();

            playClickSound();

            if (genreReady === true && actionReady === true && thingsReady === true && goalsReady === true) {
                genreReady = false;
                actionReady = false;
                thingsReady = false;
                goalsReady = false;
                rulesReady = false;

                setTimeout(playSlotsSound, 300);

                pick();
            }

            return false;
        });
    }

    function playSlotsSound() {
        if (objs.gameIdeaStandalone) {
            slotsSound.play();
        }
    }

    function playClickSound() {
        if (objs.gameIdeaStandalone) {
            clickSound.play();
        }
    }

    function pick() {
        genre = shuffle(genre);
        action = shuffle(action);
        things = shuffle(things);
        goals = shuffle(goals);
        rules = shuffle(rules);

        randomGenre();
        randomAction();
        randomThings();
        randomGoals();

        if (objs.rules) {
            randomRules();
        }
    }

    function cleanElement(str) {
        if (objs.gameIdeaStandalone) {
            var index = str.indexOf('to ');
            if (index === 0) {
                str = str.replace('to ', '');
            }

            var res = str.split(" ");
            var lastWord = res[res.length - 1];

            if (res.length > 1) {
                if (lastWord === 'with' || lastWord === 'on' || lastWord === 'involving' || lastWord === 'in' || lastWord === 'at' || lastWord === 'from') {
                    res.pop();
                    str = res.join(" ");
                }
            }
        }

        return str;
    }

    function shuffle(array) {
        var currentIndex = array.length, temporaryValue, randomIndex;
        while (0 !== currentIndex) {
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;
            temporaryValue = array[currentIndex];
            array[currentIndex] = array[randomIndex];
            array[randomIndex] = temporaryValue;
        }

        return array;
    }

    function setVerticalPosition(el) {
        el.style.top = "34px";
        if (el.offsetHeight > 120) {
            el.style.top = "-5px";
        }
        else if (el.offsetHeight < 75) {
            el.style.top = "34px";
        }
        else {
            el.style.top = "14px";
        }
    }

    return {
        Init: init
    };
}());