import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Route | account/forgot-password', function(hooks) {
  setupTest(hooks);

  test('it exists', function(assert) {
    let route = this.owner.lookup('route:account/forgot-password');
    assert.ok(route);
  });
});
