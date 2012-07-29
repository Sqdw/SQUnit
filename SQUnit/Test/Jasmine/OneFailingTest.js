describe('one failing test suite', function () {
    it('fails once', function () {
        expect(true).toBe(false);
    });

    it('foo', function() { expect(true).toBe(true); });
});
