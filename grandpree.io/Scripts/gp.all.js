var gp = gp || {};

gp.Prediction = function () {
    var self = this;
    self.DriverId = ko.observable();
    self.Grid = ko.numericObservable();
    self.Retire = ko.numericObservable();
    self.Podium = ko.numericObservable();
    self.total = ko.computed(function () {
        return parseInt(self.Grid() || 0) + parseInt(self.Podium() || 0) + parseInt(self.Retire() || 0);
    });
};

gp.PredictionViewModel = function (context) {

    var self = this;

    self.races = context.races;
    self.drivers = context.drivers;
    self.users = context.users;
    self.errorMessages = ko.observableArray([]);
    self.modelStatus = ko.observable("new");
    self.isDirty = ko.isComputed(function () {
        return self.modelStatus() != "saved";
    });

    self.selectedUser = ko.observable();
    self.selectedRace = ko.observable();
    self.predictions = ko.observableArray([]);
    self.totalPoints = ko.computed(function () {
        var total = 0;

        ko.utils.arrayForEach(self.predictions(), function (item) {


            var values = [];
            values.push(parseFloat(item.Grid() || 0));
            values.push(parseFloat(item.Podium() || 0));
            values.push(parseFloat(item.Retire() || 0));

            for (var val in values) {
                total += values[val];
            }
        });


        return total;

    });

    self.pointsBreakdown = ko.computed(function () {
        var values = {
            Grid: 0,
            Retire: 0,
            Podium: 0
        };

        ko.utils.arrayForEach(self.predictions(), function (item) {

            values.Grid += (parseFloat(item.Grid() || 0));
            values.Podium += (parseFloat(item.Podium() || 0));
            values.Retire += (parseFloat(item.Retire() || 0));

        });

        return values;
    });



    self.readyToSubmit = ko.computed(function () {

        self.errorMessages.removeAll();

        if (self.totalPoints() < 6) {
            return false;
        }


        if (self.totalPoints() > 10) {
            self.errorMessages.push({
                error: "You have used more than the available 10 points - you used : " + self.totalPoints()
            });
            return false;
        }

        var grid = self.pointsBreakdown().Grid;
        var gridValid = grid >= 1 && grid <= 7;
        if (!gridValid) {
            self.errorMessages.push({ error: "Grid points invalid need between 1 and 7 points - currently got : " + grid });
        }

        var podium = self.pointsBreakdown().Podium;
        var podiumValid = podium >= 1 && podium <= 7;
        if (!podiumValid) {
            self.errorMessages.push({ error: "Podium points invalid need between 1 and 7 points - currently got : " + podium });
        }

        var retire = self.pointsBreakdown().Retire;
        var retireValid = retire >= 2 && retire <= 8;
        if (!retireValid) {
            self.errorMessages.push({
                error: "Retire points invalid need between 2 and 8 points - currently got : " + retire
            });
        };

        if (self.totalPoints() < 10 && self.errorMessages().length == 0) {
            self.errorMessages.push({
                error: "You haven't use all 10 points yet - you only used : " + self.totalPoints()
            });
        }

        var result = self.totalPoints() == 10;
        return result
            && gridValid
            && podiumValid
            && retireValid;
    });

    self.addPrediction = function () {
        self.predictions.push(new gp.Prediction());
        self.modelStatus("dirty");
    };

    self.clearPredictions = function () {
        self.predictions.removeAll();
    };

    self.removePrediction = function (prediction) {
        self.predictions.remove(prediction);
    };

    self.submit = function () {
        var pred = ko.toJS(self.predictions);

        var d = {
            UserId: self.selectedUser(),
            RaceId: self.selectedRace(),
            Predictions: pred
        };

        $.post("/api/predictions", d);
        self.modelStatus("saved");
    };

    self.selectedRace.subscribe(function (newValue) {
        if (self.isDirty) {
            self.submit();
        }

        self.clearPredictions();
        self.modelStatus("new");

        $.get("/api/predictions/?userName=" + self.selectedUser() + "&raceId=" + self.selectedRace(), function (result) {
            //alert(JSON.stringify(result));

            for (var pred in result.Predictions) {
                var prediction = result.Predictions[pred];

                var racePrediction = new gp.Prediction();
                racePrediction.DriverId(prediction.DriverId);
                racePrediction.Grid(prediction.Grid);
                racePrediction.Podium(prediction.Podium);
                racePrediction.Retire(prediction.Retire);
                self.predictions.push(racePrediction);
            }
        });
    });
};