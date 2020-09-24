import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Route | vendors/loading', function(hooks) {
  setupTest(hooks);

  test('it exists', function(assert) {
    let route = this.owner.lookup('route:vendors/loading');
    assert.ok(route);
  });
});
