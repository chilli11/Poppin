import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Controller | vendors/vendor', function(hooks) {
  setupTest(hooks);

  // Replace this with your real tests.
  test('it exists', function(assert) {
    let controller = this.owner.lookup('controller:vendors/vendor');
    assert.ok(controller);
  });
});
