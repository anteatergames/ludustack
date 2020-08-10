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

    var genre = ['action', 'arcade', 'educational', 'top-down', 'adventure', 'strategy', 'RTS', 'turn-based strategy', 'role-playing', 'platformer', 'puzzle', 'visual novel', 'social media', 'mobile', 'browser', 'indie', 'experimental', 'student project', 'artsy'];

    var action = ['escape', 'go to war with', 'wage war on', 'unite', 'lead', 'build', 'destroy', 'conquer', 'invade', 'colonize', 'discover', 'explore', 'trade with', 'lead the rebels in', 'make peace with', 'investigate', 'rename', 'collect gold from', 'collect crystals from', 'mine ore from', 'align', 'click on', 'match', 'throw', 'toss', 'fire pellets at', 'control', 'touch', 'stack', 'guess', 'memorize', 'rotate', 'swap', 'slide', 'avoid', 'drag and drop', 'tickle', 'race', 'challenge', 'collect', 'draw', 'unlock', 'cook', 'break', 'solve puzzles involving', 'collect', 'juggle'];

    var things = ['countries', 'nations', 'dragons', 'castles', 'cities', 'strongholds', 'towers', 'dungeons', 'citadels', 'kingdoms', 'unknown worlds', 'other worlds', 'parallel worlds', 'other dimensions', 'alien worlds', 'heaven', 'hell', 'mythological places', 'historical places', 'islands', 'sanctuaries', 'temples', 'ruins', 'factories', 'caves', 'gems', 'diamonds', 'gold nuggets', 'bricks', 'bubbles', 'squares', 'triangles', 'treasure', 'blobs', 'kitchen appliances', 'nondescript fruits', 'animals', 'birds', 'baby animals', 'farm animals', 'exotic fruits', 'sentient plants', 'your friends', 'shapes', 'jewels', 'letters', 'words', 'numbers', 'tokens', 'coins', 'eggs', 'hats', 'candy', 'chocolate', 'shoes', 'clothing items', 'princesses', 'blocks', 'cubes', 'asteroids', 'stars', 'balls', 'spheres', 'magnets', 'riddles'];

    var goals = ['to win', 'for glory', 'in the name of love', 'to live forever', 'to rule the world', 'to form an empire', 'to win points', 'to reach the highscore', 'to unlock bonus items', 'to earn tokens', 'to unlock the next level'];

    var rules = ['avoid enemies', 'limited inventory', 'cant thing twice', 'one life only', 'must not be seen', 'cant touch the floor'];

    function setSelectors() {
        selectors.btnGenerateGameIdea = '#btnGenerateGameIdea';
        selectors.genre = document.querySelector('.game-idea-genre');
        selectors.action = document.querySelector('.game-idea-action');
        selectors.things = document.querySelector('.game-idea-things');
        selectors.goals = document.querySelector('.game-idea-goals');
        selectors.rules = document.querySelector('.game-idea-rules');
    }

    function cacheObjs() {
        objs.btnGenerateGameIdea = document.querySelector(selectors.btnGenerateGameIdea);
    }

    function randomGenre() {
        selectors.genre.classList.add("picking");
        pickingGenreInterval = setInterval(changeGenre, interval);

        stopGenreInterval = setInterval(stopPigkingGenre, stopInterval);
    }

    function changeGenre() {
        selectors.genre.textContent = genre[firstGenre];
        firstGenre = (firstGenre + 1) % genre.length;
    }

    function stopPigkingGenre() {
        selectors.genre.classList.remove("picking");
        clearInterval(pickingGenreInterval);
        clearInterval(stopGenreInterval);

        genreReady = true;
    }

    function randomAction() {
        selectors.action.classList.add("picking");
        pickingActionInterval = setInterval(changeAction, interval);

        stopActionInterval = setInterval(stopPigkingAction, stopInterval);
    }

    function changeAction() {
        selectors.action.textContent = action[firstAction];
        firstAction = (firstAction + 1) % action.length;
    }

    function stopPigkingAction() {
        selectors.action.classList.remove("picking");
        clearInterval(pickingActionInterval);
        clearInterval(stopActionInterval);

        actionReady = true;
    }

    function randomThings() {
        selectors.things.classList.add("picking");
        pickingThingsInterval = setInterval(changeThings, interval);

        stopThingsInterval = setInterval(stopPigkingThings, stopInterval);
    }

    function changeThings() {
        selectors.things.textContent = things[firstThing];
        firstThing = (firstThing + 1) % things.length;
    }

    function stopPigkingThings() {
        selectors.things.classList.remove("picking");
        clearInterval(pickingThingsInterval);
        clearInterval(stopThingsInterval);

        thingsReady = true;
    }

    function randomGoals() {
        selectors.goals.classList.add("picking");
        pickingGoalsInterval = setInterval(changeGoals, interval);

        stopGoalsInterval = setInterval(stopPigkingGoals, stopInterval);
    }

    function changeGoals() {
        selectors.goals.textContent = goals[firstGoal];
        firstGoal = (firstGoal + 1) % goals.length;
    }

    function stopPigkingGoals() {
        selectors.goals.classList.remove("picking");
        clearInterval(pickingGoalsInterval);
        clearInterval(stopGoalsInterval);

        goalsReady = true;
    }

    function randomRules() {
        selectors.rules.classList.add("picking");
        pickingRulesInterval = setInterval(changeRules, interval);

        stopRulesInterval = setInterval(stopPigkingRules, stopInterval);
    }

    function changeRules() {
        selectors.rules.textContent = rules[firstRule];
        firstRule = (firstRule + 1) % rules.length;
    }

    function stopPigkingRules() {
        selectors.rules.classList.remove("picking");
        clearInterval(pickingRulesInterval);
        clearInterval(stopRulesInterval);

        rulesReady = true;
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        clickSound = new Audio("/sounds/click.mp3");
        slotsSound = new Audio("/sounds/gameideagenerator.mp3");

        playSlotsSound();

        pick();
    }

    function bindAll() {
        bindGenerateClick();
    }

    function bindGenerateClick() {
        objs.btnGenerateGameIdea.addEventListener('click', function (e) {
            e.preventDefault();

            if (genreReady === true && actionReady === true && thingsReady === true && goalsReady === true) {
                genreReady = false;
                actionReady = false;
                thingsReady = false;
                goalsReady = false;
                rulesReady = false;

                clickSound.play();

                setTimeout(playSlotsSound, 300);

                pick();
            }

            return false;
        });
    }

    function playSlotsSound() {
        slotsSound.play();
    }

    function pick() {
        randomGenre();
        randomAction();
        randomThings();
        randomGoals();
        randomRules();
    }

    return {
        Init: init
    };
}());