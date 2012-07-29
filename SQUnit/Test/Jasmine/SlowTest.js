describe("Slow test suite", function () {
    it("takes one second to complete", function () {
        var done = false;
        runs(function() {
            setTimeout(function() { done = true; }, 1000);
        });
        waitsFor(function () { return done; }, 2000);
    });
});