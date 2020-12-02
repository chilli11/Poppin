import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Controller | admin/categories', function(hooks) {
  setupTest(hooks);

  // Replace this with your real tests.
  test('it exists', function(assert) {
    let controller = this.owner.lookup('controller:admin/categories');
    assert.ok(controller);
  });
});
