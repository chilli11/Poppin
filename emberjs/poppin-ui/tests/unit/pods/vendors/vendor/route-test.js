import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Route | vendors/vendor', function(hooks) {
  setupTest(hooks);

  test('it exists', function(assert) {
    let route = this.owner.lookup('route:vendors/vendor');
    assert.ok(route);
  });
});
